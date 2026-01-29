using ICMarket.Application.Mappings;
using ICMarket.Domain.Entities;

namespace ICMarket.UnitTests.Mappings;

[TestFixture]
public class BlockchainDataMappingExtensionsTests
{
	[Test]
	public void ToDto_ShouldMapAllProperties()
	{
		var entity = new BlockchainData
		{
			Id = Guid.NewGuid(),
			CreatedAt = DateTime.UtcNow,
			Name = "BTC.main",
			Height = 800000,
			Hash = "0000000000000000000abc",
			Time = "2024-01-01T00:00:00Z",
			LatestUrl = "https://api.blockcypher.com/v1/btc/main/blocks/0000000000000000000abc",
			PreviousHash = "0000000000000000000def",
			PreviousUrl = "https://api.blockcypher.com/v1/btc/main/blocks/0000000000000000000def",
			PeerCount = 250,
			UnconfirmedCount = 1500,
			LastForkHeight = 799999,
			LastForkHash = "0000000000000000000ghi",
			HighFeePerKb = 50000,
			MediumFeePerKb = 25000,
			LowFeePerKb = 10000,
			HighGasPrice = null,
			MediumGasPrice = null,
			LowGasPrice = null,
			HighPriorityFee = null,
			MediumPriorityFee = null,
			LowPriorityFee = null,
			BaseFee = null
		};

		var dto = entity.ToDto();

		Assert.Multiple(() =>
		{
			Assert.That(dto.Id, Is.EqualTo(entity.Id));
			Assert.That(dto.CreatedAt, Is.EqualTo(entity.CreatedAt));
			Assert.That(dto.Name, Is.EqualTo(entity.Name));
			Assert.That(dto.Height, Is.EqualTo(entity.Height));
			Assert.That(dto.Hash, Is.EqualTo(entity.Hash));
			Assert.That(dto.Time, Is.EqualTo(entity.Time));
			Assert.That(dto.LatestUrl, Is.EqualTo(entity.LatestUrl));
			Assert.That(dto.PreviousHash, Is.EqualTo(entity.PreviousHash));
			Assert.That(dto.PreviousUrl, Is.EqualTo(entity.PreviousUrl));
			Assert.That(dto.PeerCount, Is.EqualTo(entity.PeerCount));
			Assert.That(dto.UnconfirmedCount, Is.EqualTo(entity.UnconfirmedCount));
			Assert.That(dto.LastForkHeight, Is.EqualTo(entity.LastForkHeight));
			Assert.That(dto.LastForkHash, Is.EqualTo(entity.LastForkHash));
			Assert.That(dto.HighFeePerKb, Is.EqualTo(entity.HighFeePerKb));
			Assert.That(dto.MediumFeePerKb, Is.EqualTo(entity.MediumFeePerKb));
			Assert.That(dto.LowFeePerKb, Is.EqualTo(entity.LowFeePerKb));
			Assert.That(dto.HighGasPrice, Is.Null);
			Assert.That(dto.MediumGasPrice, Is.Null);
			Assert.That(dto.LowGasPrice, Is.Null);
		});
	}

	[Test]
	public void ToDto_ShouldMapEthGasFields()
	{
		var entity = new BlockchainData
		{
			Id = Guid.NewGuid(),
			Name = "ETH.main",
			Height = 19000000,
			Hash = "abc123",
			Time = "2024-01-01T00:00:00Z",
			HighGasPrice = 30000000000,
			MediumGasPrice = 20000000000,
			LowGasPrice = 10000000000,
			HighPriorityFee = 2000000000,
			MediumPriorityFee = 1500000000,
			LowPriorityFee = 1000000000,
			BaseFee = 15000000000
		};

		var dto = entity.ToDto();

		Assert.Multiple(() =>
		{
			Assert.That(dto.HighGasPrice, Is.EqualTo(30000000000));
			Assert.That(dto.MediumGasPrice, Is.EqualTo(20000000000));
			Assert.That(dto.LowGasPrice, Is.EqualTo(10000000000));
			Assert.That(dto.HighPriorityFee, Is.EqualTo(2000000000));
			Assert.That(dto.MediumPriorityFee, Is.EqualTo(1500000000));
			Assert.That(dto.LowPriorityFee, Is.EqualTo(1000000000));
			Assert.That(dto.BaseFee, Is.EqualTo(15000000000));
			Assert.That(dto.HighFeePerKb, Is.Null);
			Assert.That(dto.MediumFeePerKb, Is.Null);
			Assert.That(dto.LowFeePerKb, Is.Null);
		});
	}

	[Test]
	public void ToDtoList_ShouldMapAllEntities()
	{
		var entities = new List<BlockchainData>
		{
			new() { Id = Guid.NewGuid(), Name = "BTC.main", Height = 800000, Hash = "abc" },
			new() { Id = Guid.NewGuid(), Name = "ETH.main", Height = 19000000, Hash = "def" },
			new() { Id = Guid.NewGuid(), Name = "LTC.main", Height = 2500000, Hash = "ghi" }
		};

		var dtos = entities.ToDtoList().ToList();

		Assert.That(dtos, Has.Count.EqualTo(3));
		Assert.That(dtos[0].Name, Is.EqualTo("BTC.main"));
		Assert.That(dtos[1].Name, Is.EqualTo("ETH.main"));
		Assert.That(dtos[2].Name, Is.EqualTo("LTC.main"));
	}

	[Test]
	public void ToDtoList_EmptyCollection_ShouldReturnEmpty()
	{
		var entities = Enumerable.Empty<BlockchainData>();

		var dtos = entities.ToDtoList().ToList();

		Assert.That(dtos, Is.Empty);
	}
}
