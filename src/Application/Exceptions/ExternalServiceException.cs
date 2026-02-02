namespace ICMarket.Application.Exceptions;

/// <summary>
/// Thrown when an external service (e.g., BlockCypher API) fails to respond or returns an error.
/// Mapped to HTTP 502 Bad Gateway by the global exception handler.
/// </summary>
public class ExternalServiceException : ApplicationLayerException
{
	public string ServiceName { get; }

	public ExternalServiceException(string serviceName, string message, Exception innerException)
		: base(message, innerException)
	{
		ServiceName = serviceName;
	}

	public ExternalServiceException(string serviceName, string message)
		: base(message)
	{
		ServiceName = serviceName;
	}
}
