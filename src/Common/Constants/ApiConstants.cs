namespace ICMarket.Common.Constants;

/// <summary>
/// Shared constants for API route paths, CORS policy names, and configuration section keys.
/// </summary>
public static class ApiConstants
{
	public static class Routes
	{
		public const string Blockchain = "api/blockchain";
		public const string Health = "/health";
	}

	public static class Cors
	{
		public const string AllowAll = "AllowAll";
	}

	public static class Configuration
	{
		public const string Blockcypher = "Blockcypher";
		public const string BaseUrl = "BaseUrl";
		public const string Endpoints = "Endpoints";
	}
}