using System.Net;
using System.Text.Json;
using ICMarket.IntegrationTests.Infrastructure;

namespace ICMarket.IntegrationTests;

public class SwaggerTests
{
	private CustomWebApplicationFactory _factory = null!;
	private HttpClient _client = null!;

	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		_factory = new CustomWebApplicationFactory();
		_client = _factory.CreateClient();
	}

	[OneTimeTearDown]
	public void OneTimeTearDown()
	{
		_client.Dispose();
		_factory.Dispose();
	}

	[Test]
	public async Task SwaggerJson_ReturnsOk()
	{
		var response = await _client.GetAsync("/swagger/v1/swagger.json");

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
	}

	[Test]
	public async Task SwaggerJson_ReturnsValidJson()
	{
		var response = await _client.GetAsync("/swagger/v1/swagger.json");
		var content = await response.Content.ReadAsStringAsync();

		Assert.DoesNotThrow(() => JsonDocument.Parse(content));
	}

	[Test]
	public async Task SwaggerJson_ContainsBlockchainEndpoints()
	{
		var response = await _client.GetAsync("/swagger/v1/swagger.json");
		var content = await response.Content.ReadAsStringAsync();
		var doc = JsonDocument.Parse(content);
		var paths = doc.RootElement.GetProperty("paths");

		Assert.That(paths.TryGetProperty("/api/blockchain", out _), Is.True);
		Assert.That(paths.TryGetProperty("/api/blockchain/fetch", out _), Is.True);
		Assert.That(paths.TryGetProperty("/api/blockchain/{name}", out _), Is.True);
	}

	[Test]
	public async Task SwaggerUI_ReturnsOk()
	{
		var response = await _client.GetAsync("/swagger/index.html");

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
	}
}
