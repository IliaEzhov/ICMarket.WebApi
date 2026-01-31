using ICMarket.Domain.Entities;

namespace ICMarket.Application.Interfaces;

/// <summary>
/// Service interface for fetching blockchain network data from external APIs.
/// </summary>
public interface IBlockchainService
{
	/// <summary>
	/// Fetches blockchain data from all configured endpoints in parallel.
	/// </summary>
	/// <returns>A collection of <see cref="BlockchainData"/> entities from each endpoint.</returns>
	Task<IEnumerable<BlockchainData>> FetchAllBlockchainDataAsync(CancellationToken cancellationToken = default);
}
