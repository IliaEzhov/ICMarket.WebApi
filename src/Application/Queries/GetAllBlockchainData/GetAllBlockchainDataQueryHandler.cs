using ICMarket.Application.DTOs;
using ICMarket.Application.Mappings;
using ICMarket.Domain.Interfaces;
using MediatR;

namespace ICMarket.Application.Queries.GetAllBlockchainData;

public class GetAllBlockchainDataQueryHandler : IRequestHandler<GetAllBlockchainDataQuery, IEnumerable<BlockchainDataDto>>
{
	private readonly IBlockchainDataRepository _repository;

	public GetAllBlockchainDataQueryHandler(IBlockchainDataRepository repository)
	{
		_repository = repository;
	}

	public async Task<IEnumerable<BlockchainDataDto>> Handle(GetAllBlockchainDataQuery request, CancellationToken cancellationToken)
	{
		var data = await _repository.GetAllAsync(cancellationToken);
		return data.ToDtoList();
	}
}
