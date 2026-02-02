namespace ICMarket.Application.Exceptions;

/// <summary>
/// Abstract base class for all application-layer exceptions.
/// Derived exceptions are mapped to specific HTTP status codes by the global exception handler.
/// </summary>
public abstract class ApplicationLayerException : Exception
{
	protected ApplicationLayerException(string message) : base(message) { }

	protected ApplicationLayerException(string message, Exception innerException) : base(message, innerException) { }
}
