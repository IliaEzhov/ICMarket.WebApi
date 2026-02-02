using ICMarket.Common.Constants;
using ICMarket.Domain.Entities;

namespace ICMarket.IntegrationTests.Infrastructure;

public static class TestDataFactory
{
	public static List<BlockchainData> CreateSampleBlockchainData()
	{
		return new List<BlockchainData>
		{
			new()
			{
				Id = Guid.NewGuid(),
				CreatedAt = DateTime.UtcNow,
				Name = BlockchainConstants.Names.BtcMain,
				Height = 800000,
				Hash = "0000000000000000000abc",
				Time = "2024-01-01T00:00:00Z",
				LatestUrl = "https://api.blockcypher.com/v1/btc/main/blocks/abc",
				PreviousHash = "0000000000000000000def",
				PreviousUrl = "https://api.blockcypher.com/v1/btc/main/blocks/def",
				PeerCount = 250,
				UnconfirmedCount = 1500,
				LastForkHeight = 799999,
				LastForkHash = "0000000000000000000ghi",
				HighFeePerKb = 50000,
				MediumFeePerKb = 25000,
				LowFeePerKb = 10000
			},
			new()
			{
				Id = Guid.NewGuid(),
				CreatedAt = DateTime.UtcNow,
				Name = BlockchainConstants.Names.EthMain,
				Height = 19000000,
				Hash = "0xabc123",
				Time = "2024-01-01T00:00:00Z",
				LatestUrl = "https://api.blockcypher.com/v1/eth/main/blocks/abc123",
				PreviousHash = "0xdef456",
				PreviousUrl = "https://api.blockcypher.com/v1/eth/main/blocks/def456",
				PeerCount = 150,
				UnconfirmedCount = 500,
				LastForkHeight = 18999999,
				LastForkHash = "0xghi789",
				HighGasPrice = 30000000000,
				MediumGasPrice = 20000000000,
				LowGasPrice = 10000000000
			},
			new()
			{
				Id = Guid.NewGuid(),
				CreatedAt = DateTime.UtcNow,
				Name = BlockchainConstants.Names.LtcMain,
				Height = 2600000,
				Hash = "ltchash123",
				Time = "2024-01-01T00:00:00Z",
				LatestUrl = "https://api.blockcypher.com/v1/ltc/main/blocks/ltchash123",
				PreviousHash = "ltchash122",
				PreviousUrl = "https://api.blockcypher.com/v1/ltc/main/blocks/ltchash122",
				PeerCount = 100,
				UnconfirmedCount = 200,
				LastForkHeight = 2599999,
				LastForkHash = "ltcforkhash",
				HighFeePerKb = 20000,
				MediumFeePerKb = 10000,
				LowFeePerKb = 5000
			},
			new()
			{
				Id = Guid.NewGuid(),
				CreatedAt = DateTime.UtcNow,
				Name = BlockchainConstants.Names.DashMain,
				Height = 2000000,
				Hash = "dashhash123",
				Time = "2024-01-01T00:00:00Z",
				LatestUrl = "https://api.blockcypher.com/v1/dash/main/blocks/dashhash123",
				PreviousHash = "dashhash122",
				PreviousUrl = "https://api.blockcypher.com/v1/dash/main/blocks/dashhash122",
				PeerCount = 80,
				UnconfirmedCount = 100,
				LastForkHeight = 1999999,
				LastForkHash = "dashforkhash",
				HighFeePerKb = 15000,
				MediumFeePerKb = 7500,
				LowFeePerKb = 3000
			},
			new()
			{
				Id = Guid.NewGuid(),
				CreatedAt = DateTime.UtcNow,
				Name = BlockchainConstants.Names.BtcTest3,
				Height = 2500000,
				Hash = "testhash123",
				Time = "2024-01-01T00:00:00Z",
				LatestUrl = "https://api.blockcypher.com/v1/btc/test3/blocks/testhash123",
				PreviousHash = "testhash122",
				PreviousUrl = "https://api.blockcypher.com/v1/btc/test3/blocks/testhash122",
				PeerCount = 50,
				UnconfirmedCount = 300,
				LastForkHeight = 2499999,
				LastForkHash = "testforkhash",
				HighFeePerKb = 5000,
				MediumFeePerKb = 2500,
				LowFeePerKb = 1000
			}
		};
	}
}
