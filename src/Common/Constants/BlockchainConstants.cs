namespace ICMarket.Common.Constants;

/// <summary>
/// Constants for supported blockchain identifiers, BlockCypher API endpoint paths,
/// and API base URL.
/// </summary>
public static class BlockchainConstants
{
	public static class Names
	{
		public const string EthMain = "ETH.main";
		public const string DashMain = "DASH.main";
		public const string BtcMain = "BTC.main";
		public const string BtcTest3 = "BTC.test3";
		public const string LtcMain = "LTC.main";

		public static readonly string[] All = { EthMain, DashMain, BtcMain, BtcTest3, LtcMain };
	}

	public static class Endpoints
	{
		public const string EthMain = "/v1/eth/main";
		public const string DashMain = "/v1/dash/main";
		public const string BtcMain = "/v1/btc/main";
		public const string BtcTest3 = "/v1/btc/test3";
		public const string LtcMain = "/v1/ltc/main";

		public static readonly string[] All = { EthMain, DashMain, BtcMain, BtcTest3, LtcMain };
	}

	public static class Api
	{
		public const string BaseUrl = "https://api.blockcypher.com";
	}
}