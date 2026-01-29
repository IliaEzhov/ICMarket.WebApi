using ICMarket.Application.DTOs;
using MediatR;

namespace ICMarket.Application.Commands.FetchAndStoreBlockchainData;

public record FetchAndStoreBlockchainDataCommand : IRequest<IEnumerable<BlockchainDataDto>>;
