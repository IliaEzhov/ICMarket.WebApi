namespace ICMarket.Application.Interfaces;

/// <summary>
/// Marker interface for MediatR requests whose responses should be cached.
/// Implement on query records to enable automatic caching via <see cref="Behaviors.CachingBehavior{TRequest,TResponse}"/>.
/// </summary>
public interface ICacheableQuery
{
	/// <summary>
	/// Unique cache key identifying this query and its parameters.
	/// </summary>
	string CacheKey { get; }

	/// <summary>
	/// Duration to cache the response before expiration.
	/// </summary>
	TimeSpan CacheDuration { get; }
}
