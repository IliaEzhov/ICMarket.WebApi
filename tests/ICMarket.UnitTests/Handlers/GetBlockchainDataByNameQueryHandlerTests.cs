using ICMarket.Application.Queries.GetBlockchainDataByName;
using ICMarket.Domain.Entities;
using ICMarket.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace ICMarket.UnitTests.Handlers;

[TestFixture]
public class GetBlockchainDataByNameQueryHandlerTests
{
	private Mock<IBlockchainDataRepository> _repositoryMock;
	private GetBlockchainDataByNameQueryHandler _handler;

	[SetUp]
	public void SetUp()
	{
		_repositoryMock = new Mock<IBlockchainDataRepository>();
		_handler = new GetBlockchainDataByNameQueryHandler(_repositoryMock.Object, Mock.Of<ILogger<GetBlockchainDataByNameQueryHandler>>());
	}

	[Test]
	public async Task Handle_ShouldReturnPaginatedDataForSpecificBlockchain()
	{
		var entities = new List<BlockchainData>
		{
			new() { Id = Guid.NewGuid(), Name = "BTC.main", Height = 800000, Hash = "abc" },
			new() { Id = Guid.NewGuid(), Name = "BTC.main", Height = 800001, Hash = "def" }
		};

		_repositoryMock
			.Setup(r => r.GetByBlockchainNameAsync("btc/main", 1, 50, It.IsAny<CancellationToken>()))
			.ReturnsAsync((entities.AsEnumerable(), 2));

		var result = await _handler.Handle(new GetBlockchainDataByNameQuery("btc/main"), CancellationToken.None);

		Assert.That(result.Items.Count(), Is.EqualTo(2));
		Assert.That(result.Items.All(d => d.Name == "BTC.main"), Is.True);
		Assert.That(result.TotalCount, Is.EqualTo(2));
		Assert.That(result.Page, Is.EqualTo(1));
	}

	[Test]
	public async Task Handle_ShouldPassCorrectNameToRepository()
	{
		_repositoryMock
			.Setup(r => r.GetByBlockchainNameAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((Enumerable.Empty<BlockchainData>(), 0));

		await _handler.Handle(new GetBlockchainDataByNameQuery("eth/main"), CancellationToken.None);

		_repositoryMock.Verify(r => r.GetByBlockchainNameAsync("eth/main", 1, 50, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Test]
	public async Task Handle_NoData_ShouldReturnEmptyResult()
	{
		_repositoryMock
			.Setup(r => r.GetByBlockchainNameAsync("ltc/main", 1, 50, It.IsAny<CancellationToken>()))
			.ReturnsAsync((Enumerable.Empty<BlockchainData>(), 0));

		var result = await _handler.Handle(new GetBlockchainDataByNameQuery("ltc/main"), CancellationToken.None);

		Assert.That(result.Items, Is.Empty);
		Assert.That(result.TotalCount, Is.EqualTo(0));
	}
}
