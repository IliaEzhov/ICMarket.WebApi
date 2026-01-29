using ICMarket.Application.Queries.GetBlockchainDataByName;
using ICMarket.Domain.Entities;
using ICMarket.Domain.Interfaces;
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
		_handler = new GetBlockchainDataByNameQueryHandler(_repositoryMock.Object);
	}

	[Test]
	public async Task Handle_ShouldReturnDataForSpecificBlockchain()
	{
		var entities = new List<BlockchainData>
		{
			new() { Id = Guid.NewGuid(), Name = "BTC.main", Height = 800000, Hash = "abc" },
			new() { Id = Guid.NewGuid(), Name = "BTC.main", Height = 800001, Hash = "def" }
		};

		_repositoryMock
			.Setup(r => r.GetByBlockchainNameAsync("btc/main", It.IsAny<CancellationToken>()))
			.ReturnsAsync(entities);

		var result = (await _handler.Handle(new GetBlockchainDataByNameQuery("btc/main"), CancellationToken.None)).ToList();

		Assert.That(result, Has.Count.EqualTo(2));
		Assert.That(result.All(d => d.Name == "BTC.main"), Is.True);
	}

	[Test]
	public async Task Handle_ShouldPassCorrectNameToRepository()
	{
		_repositoryMock
			.Setup(r => r.GetByBlockchainNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Enumerable.Empty<BlockchainData>());

		await _handler.Handle(new GetBlockchainDataByNameQuery("eth/main"), CancellationToken.None);

		_repositoryMock.Verify(r => r.GetByBlockchainNameAsync("eth/main", It.IsAny<CancellationToken>()), Times.Once);
	}

	[Test]
	public async Task Handle_NoData_ShouldReturnEmptyList()
	{
		_repositoryMock
			.Setup(r => r.GetByBlockchainNameAsync("ltc/main", It.IsAny<CancellationToken>()))
			.ReturnsAsync(Enumerable.Empty<BlockchainData>());

		var result = (await _handler.Handle(new GetBlockchainDataByNameQuery("ltc/main"), CancellationToken.None)).ToList();

		Assert.That(result, Is.Empty);
	}
}
