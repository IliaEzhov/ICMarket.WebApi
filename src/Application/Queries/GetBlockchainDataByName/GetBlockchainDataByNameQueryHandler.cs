using ICMarket.Application.DTOs;
using ICMarket.Application.Mappings;
using ICMarket.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ICMarket.Application.Queries.GetBlockchainDataByName;

public class GetBlockchainDataByNameQueryHandler : IRequestHandler<GetBlockchainDataByNameQuery, IEnumerable<BlockchainDataDto>>
{
	private readonly IBlockchainDataRepository _repository;
	private readonly ILogger<GetBlockchainDataByNameQueryHandler> _logger;

	public GetBlockchainDataByNameQueryHandler(IBlockchainDataRepository repository, ILogger<GetBlockchainDataByNameQueryHandler> logger)
	{
		_repository = repository;
		_logger = logger;
	}

	public async Task<IEnumerable<BlockchainDataDto>> Handle(GetBlockchainDataByNameQuery request, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Querying blockchain data for {Name}", request.Name);
		var data = await _repository.GetByBlockchainNameAsync(request.Name, cancellationToken);
		_logger.LogInformation("Retrieved {Count} records for {Name}", data.Count(), request.Name);
		return data.ToDtoList();
	}
}
