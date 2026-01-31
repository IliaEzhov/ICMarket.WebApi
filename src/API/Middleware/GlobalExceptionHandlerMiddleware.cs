using System.Net;
using System.Text.Json;

namespace ICMarket.API.Middleware;

public class GlobalExceptionHandlerMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
	private readonly IHostEnvironment _environment;

	public GlobalExceptionHandlerMiddleware(
		RequestDelegate next,
		ILogger<GlobalExceptionHandlerMiddleware> logger,
		IHostEnvironment environment)
	{
		_next = next;
		_logger = logger;
		_environment = environment;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An unhandled exception occurred while processing {Method} {Path}",
				context.Request.Method, context.Request.Path);

			await HandleExceptionAsync(context, ex);
		}
	}

	private async Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		context.Response.ContentType = "application/json";
		context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

		var response = _environment.IsDevelopment()
			? new
			{
				error = "An unexpected error occurred.",
				detail = exception.Message,
				stackTrace = exception.StackTrace
			}
			: (object)new
			{
				error = "An unexpected error occurred."
			};

		var options = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};

		await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
	}
}
