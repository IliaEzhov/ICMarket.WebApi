using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ICMarket.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior that logs the start, completion, and duration of each request.
/// Also logs errors with elapsed time when exceptions occur during request handling.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the handler.</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="LoggingBehavior{TRequest, TResponse}"/> class.
	/// </summary>
	/// <param name="logger">The logger instance for logging request information.</param>
	public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
	{
		_logger = logger;
	}

	/// <summary>
	/// Handles the request by logging its start, invoking the next handler in the pipeline,
	/// and logging the completion with elapsed time.
	/// </summary>
	/// <param name="request">The incoming request.</param>
	/// <param name="next">The delegate to invoke the next handler in the pipeline.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The response from the next handler.</returns>
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		var requestName = typeof(TRequest).Name;

		_logger.LogInformation(
			"Handling {RequestName} {@Request}",
			requestName,
			request);

		var stopwatch = Stopwatch.StartNew();

		try
		{
			var response = await next();

			stopwatch.Stop();

			_logger.LogInformation(
				"Handled {RequestName} in {ElapsedMilliseconds}ms",
				requestName,
				stopwatch.ElapsedMilliseconds);

			return response;
		}
		catch (Exception ex)
		{
			stopwatch.Stop();

			_logger.LogError(
				ex,
				"Error handling {RequestName} after {ElapsedMilliseconds}ms",
				requestName,
				stopwatch.ElapsedMilliseconds);

			throw;
		}
	}
}