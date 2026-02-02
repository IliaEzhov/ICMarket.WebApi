using ICMarket.Application.Behaviors;
using ICMarket.Application.DTOs;
using ICMarket.Application.Interfaces;
using ICMarket.Application.Queries.GetAllBlockchainData;
using ICMarket.Application.Services;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace ICMarket.UnitTests.Behaviors;

[TestFixture]
public class CachingBehaviorTests
{
	private IMemoryCache _cache = null!;
	private CacheInvalidator _cacheInvalidator = null!;
	private Mock<ILogger<CachingBehavior<GetAllBlockchainDataQuery, PaginatedResult<BlockchainDataDto>>>> _loggerMock = null!;
	private CachingBehavior<GetAllBlockchainDataQuery, PaginatedResult<BlockchainDataDto>> _behavior = null!;

	[SetUp]
	public void SetUp()
	{
		_cache = new MemoryCache(new MemoryCacheOptions());
		_cacheInvalidator = new CacheInvalidator();
		_loggerMock = new Mock<ILogger<CachingBehavior<GetAllBlockchainDataQuery, PaginatedResult<BlockchainDataDto>>>>();
		_behavior = new CachingBehavior<GetAllBlockchainDataQuery, PaginatedResult<BlockchainDataDto>>(
			_cache, _cacheInvalidator, _loggerMock.Object);
	}

	[TearDown]
	public void TearDown()
	{
		_cache.Dispose();
	}

	[Test]
	public async Task Handle_CacheMiss_ShouldCallNextAndCacheResult()
	{
		var expectedResult = new PaginatedResult<BlockchainDataDto>
		{
			Items = new List<BlockchainDataDto> { new() { Name = "BTC.main" } },
			Page = 1,
			PageSize = 50,
			TotalCount = 1
		};
		var nextCalled = false;

		var result = await _behavior.Handle(
			new GetAllBlockchainDataQuery(1, 50),
			() =>
			{
				nextCalled = true;
				return Task.FromResult(expectedResult);
			},
			CancellationToken.None);

		Assert.That(nextCalled, Is.True);
		Assert.That(result, Is.EqualTo(expectedResult));
	}

	[Test]
	public async Task Handle_CacheHit_ShouldReturnCachedResultWithoutCallingNext()
	{
		var cachedResult = new PaginatedResult<BlockchainDataDto>
		{
			Items = new List<BlockchainDataDto> { new() { Name = "ETH.main" } },
			Page = 1,
			PageSize = 50,
			TotalCount = 1
		};
		var query = new GetAllBlockchainDataQuery(1, 50);

		// First call - cache miss
		await _behavior.Handle(
			query,
			() => Task.FromResult(cachedResult),
			CancellationToken.None);

		// Second call - cache hit
		var nextCalledOnSecond = false;
		var result = await _behavior.Handle(
			query,
			() =>
			{
				nextCalledOnSecond = true;
				return Task.FromResult(new PaginatedResult<BlockchainDataDto>());
			},
			CancellationToken.None);

		Assert.That(nextCalledOnSecond, Is.False);
		Assert.That(result, Is.EqualTo(cachedResult));
	}

	[Test]
	public async Task Handle_DifferentQueryParams_ShouldUseDifferentCacheKeys()
	{
		var result1 = new PaginatedResult<BlockchainDataDto>
		{
			Items = new List<BlockchainDataDto> { new() { Name = "Page1" } },
			Page = 1,
			PageSize = 50,
			TotalCount = 100
		};
		var result2 = new PaginatedResult<BlockchainDataDto>
		{
			Items = new List<BlockchainDataDto> { new() { Name = "Page2" } },
			Page = 2,
			PageSize = 50,
			TotalCount = 100
		};

		await _behavior.Handle(
			new GetAllBlockchainDataQuery(1, 50),
			() => Task.FromResult(result1),
			CancellationToken.None);

		await _behavior.Handle(
			new GetAllBlockchainDataQuery(2, 50),
			() => Task.FromResult(result2),
			CancellationToken.None);

		// Both should be cached independently
		var cachedResult1 = await _behavior.Handle(
			new GetAllBlockchainDataQuery(1, 50),
			() => Task.FromResult(new PaginatedResult<BlockchainDataDto>()),
			CancellationToken.None);

		var cachedResult2 = await _behavior.Handle(
			new GetAllBlockchainDataQuery(2, 50),
			() => Task.FromResult(new PaginatedResult<BlockchainDataDto>()),
			CancellationToken.None);

		Assert.That(cachedResult1.Page, Is.EqualTo(1));
		Assert.That(cachedResult2.Page, Is.EqualTo(2));
	}

	[Test]
	public async Task Handle_AfterInvalidation_ShouldCallNextAgain()
	{
		var originalResult = new PaginatedResult<BlockchainDataDto>
		{
			Items = new List<BlockchainDataDto> { new() { Name = "Old" } },
			Page = 1,
			PageSize = 50,
			TotalCount = 1
		};
		var newResult = new PaginatedResult<BlockchainDataDto>
		{
			Items = new List<BlockchainDataDto> { new() { Name = "New" } },
			Page = 1,
			PageSize = 50,
			TotalCount = 1
		};
		var query = new GetAllBlockchainDataQuery(1, 50);

		// Cache the original result
		await _behavior.Handle(query, () => Task.FromResult(originalResult), CancellationToken.None);

		// Invalidate cache
		_cacheInvalidator.InvalidateAll();

		// Should call next again (cache miss after invalidation)
		var nextCalled = false;
		var result = await _behavior.Handle(
			query,
			() =>
			{
				nextCalled = true;
				return Task.FromResult(newResult);
			},
			CancellationToken.None);

		Assert.That(nextCalled, Is.True);
		Assert.That(result.Items.First().Name, Is.EqualTo("New"));
	}

	[Test]
	public async Task Handle_NonCacheableRequest_ShouldBypassCache()
	{
		var loggerMock = new Mock<ILogger<CachingBehavior<NonCacheableQuery, PaginatedResult<BlockchainDataDto>>>>();
		var behavior = new CachingBehavior<NonCacheableQuery, PaginatedResult<BlockchainDataDto>>(
			_cache, _cacheInvalidator, loggerMock.Object);

		var nextCallCount = 0;
		var request = new NonCacheableQuery();

		await behavior.Handle(request, () => { nextCallCount++; return Task.FromResult(new PaginatedResult<BlockchainDataDto>()); }, CancellationToken.None);
		await behavior.Handle(request, () => { nextCallCount++; return Task.FromResult(new PaginatedResult<BlockchainDataDto>()); }, CancellationToken.None);

		Assert.That(nextCallCount, Is.EqualTo(2));
	}
}

public record NonCacheableQuery : IRequest<PaginatedResult<BlockchainDataDto>>;
