using System.Net;
using System.Text.Json;
using ICMarket.Application.Exceptions;

namespace ICMarket.API.Middleware;

/// <summary>
/// Middleware that catches all unhandled exceptions and returns a structured JSON error response.
/// Maps custom application exceptions to appropriate HTTP status codes.
/// In Development, includes exception details; in Production, returns a generic error message.
/// </summary>
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
		var (statusCode, title) = exception switch
		{
			NotFoundException => (HttpStatusCode.NotFound, "The requested resource was not found."),
			ExternalServiceException => (HttpStatusCode.BadGateway, "An external service is unavailable."),
			_ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
		};

		context.Response.ContentType = "application/json";
		context.Response.StatusCode = (int)statusCode;

		var response = _environment.IsDevelopment()
			? new
			{
				error = title,
				detail = exception.Message,
				stackTrace = exception.StackTrace
			}
			: (object)new
			{
				error = title
			};

		var options = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};

		await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
	}
}
