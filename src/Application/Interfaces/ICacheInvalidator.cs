using Microsoft.Extensions.Primitives;

namespace ICMarket.Application.Interfaces;

/// <summary>
/// Service for invalidating all cached query responses.
/// Uses a <see cref="CancellationChangeToken"/> pattern to evict cache entries.
/// </summary>
public interface ICacheInvalidator
{
	/// <summary>
	/// Creates a change token that is triggered when <see cref="InvalidateAll"/> is called.
	/// Attach to <see cref="Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions"/> to enable automatic eviction.
	/// </summary>
	IChangeToken CreateChangeToken();

	/// <summary>
	/// Invalidates all cached entries that were registered with a change token from <see cref="CreateChangeToken"/>.
	/// </summary>
	void InvalidateAll();
}
