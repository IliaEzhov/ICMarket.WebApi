using ICMarket.Application.DTOs;
using ICMarket.Application.Mappings;
using ICMarket.Domain.Interfaces;
using MediatR;

namespace ICMarket.Application.Queries.GetBlockchainDataByName;

public class GetBlockchainDataByNameQueryHandler : IRequestHandler<GetBlockchainDataByNameQuery, IEnumerable<BlockchainDataDto>>
{
	private readonly IBlockchainDataRepository _repository;

	public GetBlockchainDataByNameQueryHandler(IBlockchainDataRepository repository)
	{
		_repository = repository;
	}

	public async Task<IEnumerable<BlockchainDataDto>> Handle(GetBlockchainDataByNameQuery request, CancellationToken cancellationToken)
	{
		var data = await _repository.GetByBlockchainNameAsync(request.Name, cancellationToken);
		return data.ToDtoList();
	}
}
