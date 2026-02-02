using System.Net;
using System.Text.Json;
using ICMarket.FunctionalTests.Infrastructure;

namespace ICMarket.FunctionalTests;

[TestFixture]
public class ValidationTests
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
	public async Task GetByName_EmptyName_ReturnsBadRequest()
	{
		var response = await _client.GetAsync("/api/blockchain/%20");

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task GetByName_InvalidName_ReturnsBadRequestWithErrorMessages()
	{
		var response = await _client.GetAsync("/api/blockchain/INVALID.chain");

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

		var content = await response.Content.ReadAsStringAsync();
		var doc = JsonDocument.Parse(content);

		Assert.That(doc.RootElement.TryGetProperty("errors", out var errors), Is.True);
		Assert.That(errors.GetArrayLength(), Is.GreaterThan(0));
	}

	[TestCase("INVALID.chain")]
	[TestCase("bitcoin")]
	[TestCase("ethereum")]
	[TestCase("random")]
	public async Task GetByName_VariousInvalidNames_ReturnsBadRequest(string invalidName)
	{
		var response = await _client.GetAsync($"/api/blockchain/{invalidName}");

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[TestCase("ETH.main")]
	[TestCase("eth.main")]
	[TestCase("Eth.Main")]
	[TestCase("BTC.main")]
	[TestCase("btc.main")]
	public async Task GetByName_ValidNamesCaseInsensitive_ReturnsOk(string validName)
	{
		var response = await _client.GetAsync($"/api/blockchain/{validName}");

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
	}
}
