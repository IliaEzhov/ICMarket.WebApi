using ICMarket.Application.DTOs;
using MediatR;

namespace ICMarket.Application.Queries.GetBlockchainDataByName;

/// <summary>
/// CQRS query that retrieves stored blockchain data history filtered by blockchain name.
/// </summary>
/// <param name="Name">Blockchain name to filter by (e.g., "BTC.main", "ETH.main"). Case-insensitive.</param>
public record GetBlockchainDataByNameQuery(string Name) : IRequest<IEnumerable<BlockchainDataDto>>;
