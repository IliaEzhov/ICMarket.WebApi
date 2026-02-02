using ICMarket.Common.Constants;

namespace ICMarket.Infrastructure.Configuration;

/// <summary>
/// Strongly-typed configuration for the BlockCypher API, bound from the "Blockcypher" section
/// of appsettings.json via the options pattern.
/// </summary>
public class BlockcypherSettings
{
	public const string SectionName = ApiConstants.Configuration.Blockcypher;

	/// <summary>
	/// Base URL for the BlockCypher API (e.g., "https://api.blockcypher.com").
	/// </summary>
	public string BaseUrl { get; set; } = string.Empty;

	/// <summary>
	/// List of API endpoint paths to fetch (e.g., "/v1/btc/main", "/v1/eth/main").
	/// </summary>
	public List<string> Endpoints { get; set; } = new();
}
