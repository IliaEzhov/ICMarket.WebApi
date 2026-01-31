namespace ICMarket.Domain.Interfaces;

/// <summary>
/// Unit of Work interface for coordinating transactional persistence operations.
/// </summary>
public interface IUnitOfWork
{
	/// <summary>
	/// Persists all pending changes to the underlying data store.
	/// </summary>
	/// <returns>The number of state entries written to the database.</returns>
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
