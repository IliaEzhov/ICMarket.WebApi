using ICMarket.Application.DTOs;
using MediatR;

namespace ICMarket.Application.Queries.GetAllBlockchainData;

/// <summary>
/// CQRS query that retrieves all stored blockchain data history, ordered by CreatedAt descending.
/// </summary>
public record GetAllBlockchainDataQuery : IRequest<IEnumerable<BlockchainDataDto>>;
