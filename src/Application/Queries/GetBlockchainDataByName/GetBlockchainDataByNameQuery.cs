using ICMarket.Application.DTOs;
using MediatR;

namespace ICMarket.Application.Queries.GetBlockchainDataByName;

public record GetBlockchainDataByNameQuery(string Name) : IRequest<IEnumerable<BlockchainDataDto>>;
