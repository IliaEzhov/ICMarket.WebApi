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
	public async Task Handle_ShouldReturnPaginatedBlockchainData()
	{
		var entities = new List<BlockchainData>
		{
			new() { Id = Guid.NewGuid(), Name = "BTC.main", Height = 800000, Hash = "abc" },
			new() { Id = Guid.NewGuid(), Name = "ETH.main", Height = 19000000, Hash = "def" }
		};

		_repositoryMock
			.Setup(r => r.GetAllAsync(1, 50, It.IsAny<CancellationToken>()))
			.ReturnsAsync((entities.AsEnumerable(), 2));

		var result = await _handler.Handle(new GetAllBlockchainDataQuery(), CancellationToken.None);

		Assert.That(result.Items.Count(), Is.EqualTo(2));
		Assert.That(result.Items.First().Name, Is.EqualTo("BTC.main"));
		Assert.That(result.Items.Last().Name, Is.EqualTo("ETH.main"));
		Assert.That(result.TotalCount, Is.EqualTo(2));
		Assert.That(result.Page, Is.EqualTo(1));
		Assert.That(result.PageSize, Is.EqualTo(50));
	}

	[Test]
	public async Task Handle_EmptyRepository_ShouldReturnEmptyResult()
	{
		_repositoryMock
			.Setup(r => r.GetAllAsync(1, 50, It.IsAny<CancellationToken>()))
			.ReturnsAsync((Enumerable.Empty<BlockchainData>(), 0));

		var result = await _handler.Handle(new GetAllBlockchainDataQuery(), CancellationToken.None);

		Assert.That(result.Items, Is.Empty);
		Assert.That(result.TotalCount, Is.EqualTo(0));
	}

	[Test]
	public async Task Handle_ShouldCallRepositoryOnce()
	{
		_repositoryMock
			.Setup(r => r.GetAllAsync(1, 50, It.IsAny<CancellationToken>()))
			.ReturnsAsync((Enumerable.Empty<BlockchainData>(), 0));

		await _handler.Handle(new GetAllBlockchainDataQuery(), CancellationToken.None);

		_repositoryMock.Verify(r => r.GetAllAsync(1, 50, It.IsAny<CancellationToken>()), Times.Once);
	}
}
