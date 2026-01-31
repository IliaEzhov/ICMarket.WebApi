using ICMarket.Application.Queries.GetAllBlockchainData;
using ICMarket.Domain.Entities;
using ICMarket.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace ICMarket.UnitTests.Handlers;

[TestFixture]
public class GetAllBlockchainDataQueryHandlerTests
{
	private Mock<IBlockchainDataRepository> _repositoryMock;
	private GetAllBlockchainDataQueryHandler _handler;

	[SetUp]
	public void SetUp()
	{
		_repositoryMock = new Mock<IBlockchainDataRepository>();
		_handler = new GetAllBlockchainDataQueryHandler(_repositoryMock.Object, Mock.Of<ILogger<GetAllBlockchainDataQueryHandler>>());
	}

	[Test]
	public async Task Handle_ShouldReturnAllBlockchainData()
	{
		var entities = new List<BlockchainData>
		{
			new() { Id = Guid.NewGuid(), Name = "BTC.main", Height = 800000, Hash = "abc" },
			new() { Id = Guid.NewGuid(), Name = "ETH.main", Height = 19000000, Hash = "def" }
		};

		_repositoryMock
			.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(entities);

		var result = (await _handler.Handle(new GetAllBlockchainDataQuery(), CancellationToken.None)).ToList();

		Assert.That(result, Has.Count.EqualTo(2));
		Assert.That(result[0].Name, Is.EqualTo("BTC.main"));
		Assert.That(result[1].Name, Is.EqualTo("ETH.main"));
	}

	[Test]
	public async Task Handle_EmptyRepository_ShouldReturnEmptyList()
	{
		_repositoryMock
			.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(Enumerable.Empty<BlockchainData>());

		var result = (await _handler.Handle(new GetAllBlockchainDataQuery(), CancellationToken.None)).ToList();

		Assert.That(result, Is.Empty);
	}

	[Test]
	public async Task Handle_ShouldCallRepositoryOnce()
	{
		_repositoryMock
			.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(Enumerable.Empty<BlockchainData>());

		await _handler.Handle(new GetAllBlockchainDataQuery(), CancellationToken.None);

		_repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
	}
}
