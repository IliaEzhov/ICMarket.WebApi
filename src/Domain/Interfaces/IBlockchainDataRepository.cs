using ICMarket.Domain.Entities;

namespace ICMarket.Domain.Interfaces;

/// <summary>
/// Repository interface for persisting and querying <see cref="BlockchainData"/> entities.
/// </summary>
public interface IBlockchainDataRepository
{
	/// <summary>
	/// Retrieves a paginated set of blockchain data records ordered by <see cref="BaseEntity.CreatedAt"/> descending.
	/// </summary>
	Task<(IEnumerable<BlockchainData> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);

	/// <summary>
	/// Retrieves a paginated set of blockchain data records filtered by blockchain name (case-insensitive),
	/// ordered by <see cref="BaseEntity.CreatedAt"/> descending.
	/// </summary>
	Task<(IEnumerable<BlockchainData> Items, int TotalCount)> GetByBlockchainNameAsync(string name, int page, int pageSize, CancellationToken cancellationToken = default);

	/// <summary>
	/// Adds a single <see cref="BlockchainData"/> entity to the repository.
	/// </summary>
	Task AddAsync(BlockchainData entity, CancellationToken cancellationToken = default);

	/// <summary>
	/// Adds multiple <see cref="BlockchainData"/> entities to the repository in bulk.
	/// </summary>
	Task AddRangeAsync(IEnumerable<BlockchainData> entities, CancellationToken cancellationToken = default);
}
