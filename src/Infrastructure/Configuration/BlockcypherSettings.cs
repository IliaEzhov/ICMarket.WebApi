using ICMarket.Common.Constants;

namespace ICMarket.Infrastructure.Configuration;

public class BlockcypherSettings
{
	public const string SectionName = ApiConstants.Configuration.Blockcypher;

	public string BaseUrl { get; set; } = string.Empty;

	public List<string> Endpoints { get; set; } = new();
}
