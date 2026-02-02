namespace ICMarket.Application.Exceptions;

/// <summary>
/// Thrown when a requested resource could not be found.
/// Mapped to HTTP 404 Not Found by the global exception handler.
/// </summary>
public class NotFoundException : ApplicationLayerException
{
	public NotFoundException(string resourceName, object key)
		: base($"{resourceName} with key '{key}' was not found.") { }

	public NotFoundException(string message) : base(message) { }
}
