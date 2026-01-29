using ICMarket.Domain.Entities;

namespace ICMarket.Domain.Interfaces;

public interface IBlockchainDataRepository
{
	Task<IEnumerable<BlockchainData>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<IEnumerable<BlockchainData>> GetByBlockchainNameAsync(string name, CancellationToken cancellationToken = default);
	Task AddAsync(BlockchainData entity, CancellationToken cancellationToken = default);
	Task AddRangeAsync(IEnumerable<BlockchainData> entities, CancellationToken cancellationToken = default);
}
