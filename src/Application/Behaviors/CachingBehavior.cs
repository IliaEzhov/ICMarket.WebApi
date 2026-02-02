using ICMarket.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ICMarket.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior that caches responses for requests implementing <see cref="ICacheableQuery"/>.
/// Cache entries are automatically evicted when <see cref="ICacheInvalidator.InvalidateAll"/> is called.
/// </summary>
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	private readonly IMemoryCache _cache;
	private readonly ICacheInvalidator _cacheInvalidator;
	private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

	public CachingBehavior(
		IMemoryCache cache,
		ICacheInvalidator cacheInvalidator,
		ILogger<CachingBehavior<TRequest, TResponse>> logger)
	{
		_cache = cache;
		_cacheInvalidator = cacheInvalidator;
		_logger = logger;
	}

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		if (request is not ICacheableQuery cacheableQuery)
			return await next();

		var cacheKey = cacheableQuery.CacheKey;

		if (_cache.TryGetValue(cacheKey, out TResponse? cachedResponse) && cachedResponse is not null)
		{
			_logger.LogDebug("Cache hit for {CacheKey}", cacheKey);
			return cachedResponse;
		}

		_logger.LogInformation("Cache miss for {CacheKey}, executing handler", cacheKey);
		var response = await next();

		var cacheOptions = new MemoryCacheEntryOptions()
			.SetAbsoluteExpiration(cacheableQuery.CacheDuration)
			.AddExpirationToken(_cacheInvalidator.CreateChangeToken());

		_cache.Set(cacheKey, response, cacheOptions);
		_logger.LogInformation("Cached response for {CacheKey} (expires in {Duration})", cacheKey, cacheableQuery.CacheDuration);

		return response;
	}
}
