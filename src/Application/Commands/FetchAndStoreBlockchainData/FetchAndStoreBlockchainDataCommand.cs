using ICMarket.Application.DTOs;
using MediatR;

namespace ICMarket.Application.Commands.FetchAndStoreBlockchainData;

/// <summary>
/// CQRS command that triggers fetching blockchain data from all configured BlockCypher
/// endpoints and persisting the results to the database.
/// </summary>
public record FetchAndStoreBlockchainDataCommand : IRequest<IEnumerable<BlockchainDataDto>>;
