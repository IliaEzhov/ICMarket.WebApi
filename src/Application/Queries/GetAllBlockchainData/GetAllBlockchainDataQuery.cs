using ICMarket.Application.DTOs;
using MediatR;

namespace ICMarket.Application.Queries.GetAllBlockchainData;

public record GetAllBlockchainDataQuery : IRequest<IEnumerable<BlockchainDataDto>>;
