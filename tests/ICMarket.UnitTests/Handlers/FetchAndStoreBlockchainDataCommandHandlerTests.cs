using ICMarket.Application.Commands.FetchAndStoreBlockchainData;
using ICMarket.Application.Exceptions;
using ICMarket.Application.Interfaces;
using ICMarket.Domain.Entities;
using ICMarket.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace ICMarket.UnitTests.Handlers;

[TestFixture]
public class FetchAndStoreBlockchainDataCommandHandlerTests
{
	private Mock<IBlockchainService> _blockchainServiceMock;
	private Mock<IBlockchainDataRepository> _repositoryMock;
	private Mock<IUnitOfWork> _unitOfWorkMock;
	private Mock<ICacheInvalidator> _cacheInvalidatorMock;
	private FetchAndStoreBlockchainDataCommandHandler _handler;

	[SetUp]
	public void SetUp()
	{
		_blockchainServiceMock = new Mock<IBlockchainService>();
		_repositoryMock = new Mock<IBlockchainDataRepository>();
		_unitOfWorkMock = new Mock<IUnitOfWork>();
		_cacheInvalidatorMock = new Mock<ICacheInvalidator>();
		_handler = new FetchAndStoreBlockchainDataCommandHandler(
			_blockchainServiceMock.Object,
			_repositoryMock.Object,
			_unitOfWorkMock.Object,
			_cacheInvalidatorMock.Object,
			Mock.Of<ILogger<FetchAndStoreBlockchainDataCommandHandler>>());
	}

	[Test]
	public async Task Handle_ShouldFetchStoreAndReturnData()
	{
		var fetchedData = new List<BlockchainData>
		{
			new() { Id = Guid.NewGuid(), Name = "BTC.main", Height = 800000, Hash = "abc" },
			new() { Id = Guid.NewGuid(), Name = "ETH.main", Height = 19000000, Hash = "def" }
		};

		_blockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(fetchedData);

		_unitOfWorkMock
			.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(2);

		var result = (await _handler.Handle(new FetchAndStoreBlockchainDataCommand(), CancellationToken.None)).ToList();

		Assert.That(result, Has.Count.EqualTo(2));
		Assert.That(result[0].Name, Is.EqualTo("BTC.main"));
		Assert.That(result[1].Name, Is.EqualTo("ETH.main"));
	}

	[Test]
	public async Task Handle_ShouldCallAddRangeAsync()
	{
		var fetchedData = new List<BlockchainData>
		{
			new() { Id = Guid.NewGuid(), Name = "BTC.main", Height = 800000, Hash = "abc" }
		};

		_blockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(fetchedData);

		_unitOfWorkMock
			.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(1);

		await _handler.Handle(new FetchAndStoreBlockchainDataCommand(), CancellationToken.None);

		_repositoryMock.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<BlockchainData>>(), It.IsAny<CancellationToken>()), Times.Once);
	}

	[Test]
	public async Task Handle_ShouldCallSaveChangesAsync()
	{
		_blockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(Enumerable.Empty<BlockchainData>());

		_unitOfWorkMock
			.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(0);

		await _handler.Handle(new FetchAndStoreBlockchainDataCommand(), CancellationToken.None);

		_unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Test]
	public async Task Handle_NoDataFetched_ShouldReturnEmptyAndStillSave()
	{
		_blockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(Enumerable.Empty<BlockchainData>());

		_unitOfWorkMock
			.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(0);

		var result = (await _handler.Handle(new FetchAndStoreBlockchainDataCommand(), CancellationToken.None)).ToList();

		Assert.That(result, Is.Empty);
		_repositoryMock.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<BlockchainData>>(), It.IsAny<CancellationToken>()), Times.Once);
		_unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Test]
	public void Handle_WhenExternalApiFails_ShouldThrowExternalServiceException()
	{
		_blockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ThrowsAsync(new HttpRequestException("Connection refused"));

		var ex = Assert.ThrowsAsync<ExternalServiceException>(
			async () => await _handler.Handle(new FetchAndStoreBlockchainDataCommand(), CancellationToken.None));

		Assert.That(ex!.ServiceName, Is.EqualTo("BlockCypher"));
		Assert.That(ex.InnerException, Is.TypeOf<HttpRequestException>());
	}

	[Test]
	public void Handle_WhenExternalApiTimesOut_ShouldThrowExternalServiceException()
	{
		_blockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ThrowsAsync(new TaskCanceledException("Request timed out", new TimeoutException()));

		var ex = Assert.ThrowsAsync<ExternalServiceException>(
			async () => await _handler.Handle(new FetchAndStoreBlockchainDataCommand(), CancellationToken.None));

		Assert.That(ex!.ServiceName, Is.EqualTo("BlockCypher"));
	}

	[Test]
	public void Handle_WhenCancelled_ShouldNotWrapInExternalServiceException()
	{
		var cts = new CancellationTokenSource();
		cts.Cancel();

		_blockchainServiceMock
			.Setup(s => s.FetchAllBlockchainDataAsync(It.IsAny<CancellationToken>()))
			.ThrowsAsync(new OperationCanceledException());

		Assert.ThrowsAsync<OperationCanceledException>(
			async () => await _handler.Handle(new FetchAndStoreBlockchainDataCommand(), cts.Token));
	}
}
