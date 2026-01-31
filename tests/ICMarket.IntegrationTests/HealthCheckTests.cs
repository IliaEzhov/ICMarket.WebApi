using System.Net;
using ICMarket.IntegrationTests.Infrastructure;

namespace ICMarket.IntegrationTests;

[TestFixture]
public class HealthCheckTests
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
	public async Task HealthCheck_ReturnsOk()
	{
		var response = await _client.GetAsync("/health");

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
	}

	[Test]
	public async Task HealthCheck_ReturnsHealthyContent()
	{
		var response = await _client.GetAsync("/health");
		var content = await response.Content.ReadAsStringAsync();

		Assert.That(content, Is.EqualTo("Healthy"));
	}
}
