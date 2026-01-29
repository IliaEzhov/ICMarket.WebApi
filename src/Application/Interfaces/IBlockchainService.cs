using ICMarket.Domain.Entities;

namespace ICMarket.Application.Interfaces;

public interface IBlockchainService
{
	Task<IEnumerable<BlockchainData>> FetchAllBlockchainDataAsync(CancellationToken cancellationToken = default);
}
