using System.Net;
using System.Net.Http.Json;
using ICMarket.Application.DTOs;
using ICMarket.Common.Constants;
using ICMarket.Domain.Entities;
using ICMarket.FunctionalTests.Infrastructure;
using Moq;

namespace ICMarket.FunctionalTests;

[TestFixture]
public class BlockchainWorkflowTests
{
	private CustomWebApplicationFactory _factory = null!;
	private HttpClient _client = null!;

	[SetUp]
	public void SetUp()
	{
		_factory = new CustomWebApplicationFactory();
		_client = _factory.CreateClient();
	}

	[TearDown]
	public void TearDown()
	{
		_client.Dispose();
		_factory.Dispose();
	}

	[Test]
	public async Task FetchThenGetAll_ReturnsStoredData()
	{
		var sampleData = TestDataFactory.CreateSampleBlockchainData();
		_factory.BlockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(sampleData);

		var fetchResponse = await _client.PostAsync("/api/blockchain/fetch", null);
		Assert.That(fetchResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

		var getResponse = await _client.GetAsync("/api/blockchain");
		var data = await getResponse.Content.ReadFromJsonAsync<List<BlockchainDataDto>>();

		Assert.That(data, Is.Not.Null);
		Assert.That(data, Has.Count.EqualTo(5));
	}

	[Test]
	public async Task MultipleFetches_AccumulateHistory()
	{
		var sampleData = TestDataFactory.CreateSampleBlockchainData();
		var callCount = 0;
		_factory.BlockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(() =>
			{
				callCount++;
				return sampleData.Select(d => new BlockchainData
				{
					Id = Guid.NewGuid(),
					CreatedAt = DateTime.UtcNow,
					Name = d.Name,
					Height = d.Height + callCount,
					Hash = $"{d.Hash}_{callCount}",
					Time = d.Time,
					LatestUrl = d.LatestUrl,
					PreviousHash = d.PreviousHash,
					PreviousUrl = d.PreviousUrl,
					PeerCount = d.PeerCount,
					UnconfirmedCount = d.UnconfirmedCount,
					LastForkHeight = d.LastForkHeight,
					LastForkHash = d.LastForkHash
				}).ToList();
			});

		await _client.PostAsync("/api/blockchain/fetch", null);
		await _client.PostAsync("/api/blockchain/fetch", null);

		var getResponse = await _client.GetAsync("/api/blockchain");
		var data = await getResponse.Content.ReadFromJsonAsync<List<BlockchainDataDto>>();

		Assert.That(data, Is.Not.Null);
		Assert.That(data, Has.Count.EqualTo(10));
	}

	[Test]
	public async Task GetByName_FiltersCorrectly()
	{
		var sampleData = TestDataFactory.CreateSampleBlockchainData();
		_factory.BlockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(sampleData);

		await _client.PostAsync("/api/blockchain/fetch", null);

		var response = await _client.GetAsync($"/api/blockchain/{BlockchainConstants.Names.EthMain}");
		var data = await response.Content.ReadFromJsonAsync<List<BlockchainDataDto>>();

		Assert.That(data, Is.Not.Null);
		Assert.That(data, Has.Count.EqualTo(1));
		Assert.That(data![0].Name, Is.EqualTo(BlockchainConstants.Names.EthMain));
		Assert.That(data[0].HighGasPrice, Is.EqualTo(30000000000));
	}

	[Test]
	public async Task GetByName_CaseInsensitive_ReturnsData()
	{
		var sampleData = TestDataFactory.CreateSampleBlockchainData();
		_factory.BlockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(sampleData);

		await _client.PostAsync("/api/blockchain/fetch", null);

		var response = await _client.GetAsync("/api/blockchain/btc.main");
		var data = await response.Content.ReadFromJsonAsync<List<BlockchainDataDto>>();

		Assert.That(data, Is.Not.Null);
		Assert.That(data, Has.Count.EqualTo(1));
		Assert.That(data![0].Name, Is.EqualTo(BlockchainConstants.Names.BtcMain));
	}

	[Test]
	public async Task GetAll_ReturnsDataOrderedByCreatedAtDescending()
	{
		var firstBatch = new List<BlockchainData>
		{
			new()
			{
				Id = Guid.NewGuid(),
				CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
				Name = BlockchainConstants.Names.BtcMain,
				Height = 800000,
				Hash = "hash1",
				Time = "2024-01-01T00:00:00Z",
				LatestUrl = "url1",
				PreviousHash = "prev1",
				PreviousUrl = "prevurl1",
				PeerCount = 250,
				UnconfirmedCount = 1500,
				LastForkHeight = 799999,
				LastForkHash = "fork1"
			}
		};

		var secondBatch = new List<BlockchainData>
		{
			new()
			{
				Id = Guid.NewGuid(),
				CreatedAt = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc),
				Name = BlockchainConstants.Names.BtcMain,
				Height = 850000,
				Hash = "hash2",
				Time = "2024-06-01T00:00:00Z",
				LatestUrl = "url2",
				PreviousHash = "prev2",
				PreviousUrl = "prevurl2",
				PeerCount = 260,
				UnconfirmedCount = 1600,
				LastForkHeight = 849999,
				LastForkHash = "fork2"
			}
		};

		var callCount = 0;
		_factory.BlockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(() =>
			{
				callCount++;
				return callCount == 1 ? firstBatch : secondBatch;
			});

		await _client.PostAsync("/api/blockchain/fetch", null);
		await _client.PostAsync("/api/blockchain/fetch", null);

		var response = await _client.GetAsync("/api/blockchain");
		var data = await response.Content.ReadFromJsonAsync<List<BlockchainDataDto>>();

		Assert.That(data, Is.Not.Null);
		Assert.That(data, Has.Count.EqualTo(2));
		Assert.That(data![0].CreatedAt, Is.GreaterThanOrEqualTo(data[1].CreatedAt));
		Assert.That(data[0].Height, Is.EqualTo(850000));
	}

	[Test]
	public async Task CreatedAt_IsPopulatedOnFetchedData()
	{
		var sampleData = TestDataFactory.CreateSampleBlockchainData();
		_factory.BlockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(sampleData);

		var beforeFetch = DateTime.UtcNow.AddSeconds(-1);
		await _client.PostAsync("/api/blockchain/fetch", null);

		var response = await _client.GetAsync("/api/blockchain");
		var data = await response.Content.ReadFromJsonAsync<List<BlockchainDataDto>>();

		Assert.That(data, Is.Not.Null);
		Assert.That(data!.All(d => d.CreatedAt >= beforeFetch), Is.True);
	}

	[Test]
	public async Task GetByName_AfterMultipleFetches_ReturnsAllHistoryForThatName()
	{
		var callCount = 0;
		_factory.BlockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(() =>
			{
				callCount++;
				return new List<BlockchainData>
				{
					new()
					{
						Id = Guid.NewGuid(),
						CreatedAt = DateTime.UtcNow,
						Name = BlockchainConstants.Names.EthMain,
						Height = 19000000 + callCount,
						Hash = $"ethhash_{callCount}",
						Time = "2024-01-01T00:00:00Z",
						LatestUrl = "url",
						PreviousHash = "prevhash",
						PreviousUrl = "prevurl",
						PeerCount = 150,
						UnconfirmedCount = 500,
						LastForkHeight = 18999999,
						LastForkHash = "forkhash"
					}
				};
			});

		await _client.PostAsync("/api/blockchain/fetch", null);
		await _client.PostAsync("/api/blockchain/fetch", null);
		await _client.PostAsync("/api/blockchain/fetch", null);

		var response = await _client.GetAsync($"/api/blockchain/{BlockchainConstants.Names.EthMain}");
		var data = await response.Content.ReadFromJsonAsync<List<BlockchainDataDto>>();

		Assert.That(data, Is.Not.Null);
		Assert.That(data, Has.Count.EqualTo(3));
		Assert.That(data!.All(d => d.Name == BlockchainConstants.Names.EthMain), Is.True);
	}
}
