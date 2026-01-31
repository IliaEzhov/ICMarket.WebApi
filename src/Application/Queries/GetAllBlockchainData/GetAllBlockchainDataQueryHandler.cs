using ICMarket.Application.DTOs;
using ICMarket.Application.Mappings;
using ICMarket.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ICMarket.Application.Queries.GetAllBlockchainData;

public class GetAllBlockchainDataQueryHandler : IRequestHandler<GetAllBlockchainDataQuery, IEnumerable<BlockchainDataDto>>
{
	private readonly IBlockchainDataRepository _repository;
	private readonly ILogger<GetAllBlockchainDataQueryHandler> _logger;

	public GetAllBlockchainDataQueryHandler(IBlockchainDataRepository repository, ILogger<GetAllBlockchainDataQueryHandler> logger)
	{
		_repository = repository;
		_logger = logger;
	}

	public async Task<IEnumerable<BlockchainDataDto>> Handle(GetAllBlockchainDataQuery request, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Querying all blockchain data");
		var data = await _repository.GetAllAsync(cancellationToken);
		_logger.LogInformation("Retrieved {Count} blockchain records", data.Count());
		return data.ToDtoList();
	}
}
