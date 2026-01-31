namespace ICMarket.Common.Constants;

/// <summary>
/// Constants for Swagger/OpenAPI configuration: API metadata and endpoint URLs.
/// </summary>
public static class SwaggerConstants
{
	public static class ApiInfo
	{
		public const string Title = "ICMarket Blockchain API";
		public const string Version = "v1";
		public const string Description = "Web API for storing and retrieving blockchain data from BlockCypher";
	}

	public static class Endpoints
	{
		public const string SwaggerJson = "/swagger/v1/swagger.json";
		public const string SwaggerUi = "/swagger/index.html";
	}
}