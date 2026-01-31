using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ICMarket.Application.DTOs;
using ICMarket.Common.Constants;
using ICMarket.IntegrationTests.Infrastructure;
using Moq;

namespace ICMarket.IntegrationTests;

public class BlockchainEndpointTests
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
	public async Task GetAll_WhenNoData_ReturnsEmptyArray()
	{
		var response = await _client.GetAsync("/api/blockchain");

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

		var data = await response.Content.ReadFromJsonAsync<List<BlockchainDataDto>>();
		Assert.That(data, Is.Not.Null);
		Assert.That(data, Is.Empty);
	}

	[Test]
	public async Task GetAll_ReturnsOkStatusCode()
	{
		var response = await _client.GetAsync("/api/blockchain");

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
		Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
	}

	[Test]
	public async Task Fetch_ReturnsOkWithData()
	{
		var sampleData = TestDataFactory.CreateSampleBlockchainData();
		_factory.BlockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(sampleData);

		var response = await _client.PostAsync("/api/blockchain/fetch", null);

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

		var data = await response.Content.ReadFromJsonAsync<List<BlockchainDataDto>>();
		Assert.That(data, Is.Not.Null);
		Assert.That(data, Has.Count.EqualTo(sampleData.Count));
	}

	[Test]
	public async Task Fetch_StoresDataInDatabase()
	{
		var sampleData = TestDataFactory.CreateSampleBlockchainData();
		_factory.BlockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(sampleData);

		await _client.PostAsync("/api/blockchain/fetch", null);

		var response = await _client.GetAsync("/api/blockchain");
		var data = await response.Content.ReadFromJsonAsync<List<BlockchainDataDto>>();

		Assert.That(data, Is.Not.Null);
		Assert.That(data, Has.Count.EqualTo(sampleData.Count));
	}

	[Test]
	public async Task GetByName_WithValidName_ReturnsOk()
	{
		var sampleData = TestDataFactory.CreateSampleBlockchainData();
		_factory.BlockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(sampleData);

		await _client.PostAsync("/api/blockchain/fetch", null);

		var response = await _client.GetAsync($"/api/blockchain/{BlockchainConstants.Names.BtcMain}");

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

		var data = await response.Content.ReadFromJsonAsync<List<BlockchainDataDto>>();
		Assert.That(data, Is.Not.Null);
		Assert.That(data, Has.Count.EqualTo(1));
		Assert.That(data![0].Name, Is.EqualTo(BlockchainConstants.Names.BtcMain));
	}

	[Test]
	public async Task GetByName_WithInvalidName_ReturnsBadRequest()
	{
		var response = await _client.GetAsync("/api/blockchain/INVALID.chain");

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task GetByName_InvalidName_ReturnsErrorMessages()
	{
		var response = await _client.GetAsync("/api/blockchain/INVALID.chain");
		var content = await response.Content.ReadAsStringAsync();
		var doc = JsonDocument.Parse(content);

		Assert.That(doc.RootElement.TryGetProperty("errors", out var errors), Is.True);
		Assert.That(errors.GetArrayLength(), Is.GreaterThan(0));
	}

	[Test]
	public async Task Fetch_ResponseUsesCamelCaseJson()
	{
		var sampleData = TestDataFactory.CreateSampleBlockchainData();
		_factory.BlockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(sampleData);

		var response = await _client.PostAsync("/api/blockchain/fetch", null);
		var content = await response.Content.ReadAsStringAsync();

		Assert.That(content, Does.Contain("\"name\""));
		Assert.That(content, Does.Contain("\"height\""));
		Assert.That(content, Does.Contain("\"createdAt\""));
		Assert.That(content, Does.Not.Contain("\"Name\""));
		Assert.That(content, Does.Not.Contain("\"Height\""));
	}

	[TestCase("ETH.main")]
	[TestCase("DASH.main")]
	[TestCase("BTC.main")]
	[TestCase("BTC.test3")]
	[TestCase("LTC.main")]
	public async Task GetByName_AllValidNames_ReturnOk(string name)
	{
		var sampleData = TestDataFactory.CreateSampleBlockchainData();
		_factory.BlockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(sampleData);

		await _client.PostAsync("/api/blockchain/fetch", null);

		var response = await _client.GetAsync($"/api/blockchain/{name}");

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
	}
}
