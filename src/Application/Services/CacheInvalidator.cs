using ICMarket.Application.Interfaces;
using Microsoft.Extensions.Primitives;

namespace ICMarket.Application.Services;

/// <summary>
/// Thread-safe cache invalidator that uses a shared <see cref="CancellationTokenSource"/>
/// to evict all cached entries simultaneously.
/// </summary>
public class CacheInvalidator : ICacheInvalidator
{
	private CancellationTokenSource _cts = new();
	private readonly object _lock = new();

	public IChangeToken CreateChangeToken()
	{
		lock (_lock)
			return new CancellationChangeToken(_cts.Token);
	}

	public void InvalidateAll()
	{
		lock (_lock)
		{
			var old = _cts;
			_cts = new CancellationTokenSource();
			old.Cancel();
			old.Dispose();
		}
	}
}
