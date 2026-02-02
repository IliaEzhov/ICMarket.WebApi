using System.Net;
using System.Text.Json;
using ICMarket.Application.Exceptions;
using ICMarket.IntegrationTests.Infrastructure;
using Moq;

namespace ICMarket.IntegrationTests;

[TestFixture]
public class ExceptionHandlingTests
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
	public async Task Fetch_WhenExternalApiFails_Returns502BadGateway()
	{
		_factory.BlockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ThrowsAsync(new HttpRequestException("Connection refused"));

		var response = await _client.PostAsync("/api/blockchain/fetch", null);

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadGateway));
	}

	[Test]
	public async Task Fetch_WhenExternalApiFails_ReturnsExternalServiceErrorMessage()
	{
		_factory.BlockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ThrowsAsync(new HttpRequestException("Connection refused"));

		var response = await _client.PostAsync("/api/blockchain/fetch", null);
		var content = await response.Content.ReadAsStringAsync();
		var doc = JsonDocument.Parse(content);

		Assert.That(doc.RootElement.TryGetProperty("error", out var error), Is.True);
		Assert.That(error.GetString(), Is.EqualTo("An external service is unavailable."));
	}

	[Test]
	public async Task Fetch_WhenExternalApiTimesOut_Returns502BadGateway()
	{
		_factory.BlockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ThrowsAsync(new TaskCanceledException("Timed out", new TimeoutException()));

		var response = await _client.PostAsync("/api/blockchain/fetch", null);

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadGateway));
	}

	[Test]
	public async Task Fetch_WhenNotFoundExceptionOccurs_Returns404NotFound()
	{
		_factory.BlockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ThrowsAsync(new NotFoundException("BlockchainData", "unknown"));

		var response = await _client.PostAsync("/api/blockchain/fetch", null);

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task Fetch_WhenNotFoundExceptionOccurs_ReturnsNotFoundErrorMessage()
	{
		_factory.BlockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ThrowsAsync(new NotFoundException("BlockchainData", "unknown"));

		var response = await _client.PostAsync("/api/blockchain/fetch", null);
		var content = await response.Content.ReadAsStringAsync();
		var doc = JsonDocument.Parse(content);

		Assert.That(doc.RootElement.TryGetProperty("error", out var error), Is.True);
		Assert.That(error.GetString(), Is.EqualTo("The requested resource was not found."));
	}

	[Test]
	public async Task Fetch_WhenUnexpectedExceptionOccurs_Returns500InternalServerError()
	{
		_factory.BlockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ThrowsAsync(new InvalidOperationException("Something unexpected"));

		var response = await _client.PostAsync("/api/blockchain/fetch", null);

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
	}

	[Test]
	public async Task Fetch_WhenUnexpectedExceptionOccurs_ReturnsGenericErrorMessage()
	{
		_factory.BlockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ThrowsAsync(new InvalidOperationException("Something unexpected"));

		var response = await _client.PostAsync("/api/blockchain/fetch", null);
		var content = await response.Content.ReadAsStringAsync();
		var doc = JsonDocument.Parse(content);

		Assert.That(doc.RootElement.TryGetProperty("error", out var error), Is.True);
		Assert.That(error.GetString(), Is.EqualTo("An unexpected error occurred."));
	}

	[Test]
	public async Task Fetch_WhenExceptionOccurs_ResponseContentTypeIsJson()
	{
		_factory.BlockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ThrowsAsync(new HttpRequestException("Connection refused"));

		var response = await _client.PostAsync("/api/blockchain/fetch", null);

		Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
	}
}
