namespace ICMarket.Domain.Entities;

public class BlockchainData : BaseEntity
{
	public string Name { get; set; } = string.Empty;
	public long Height { get; set; }
	public string Hash { get; set; } = string.Empty;
	public string Time { get; set; } = string.Empty;
	public string LatestUrl { get; set; } = string.Empty;
	public string PreviousHash { get; set; } = string.Empty;
	public string PreviousUrl { get; set; } = string.Empty;
	public int PeerCount { get; set; }
	public int UnconfirmedCount { get; set; }
	public long LastForkHeight { get; set; }
	public string LastForkHash { get; set; } = string.Empty;

	// BTC/DASH/LTC fee fields
	public long? HighFeePerKb { get; set; }
	public long? MediumFeePerKb { get; set; }
	public long? LowFeePerKb { get; set; }

	// ETH gas fields
	public long? HighGasPrice { get; set; }
	public long? MediumGasPrice { get; set; }
	public long? LowGasPrice { get; set; }
	public long? HighPriorityFee { get; set; }
	public long? MediumPriorityFee { get; set; }
	public long? LowPriorityFee { get; set; }
	public long? BaseFee { get; set; }
}
