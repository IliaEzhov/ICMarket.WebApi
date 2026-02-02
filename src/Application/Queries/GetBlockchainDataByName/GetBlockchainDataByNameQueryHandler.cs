using ICMarket.Application.DTOs;
using ICMarket.Application.Mappings;
using ICMarket.Common.Constants;
using ICMarket.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ICMarket.Application.Queries.GetBlockchainDataByName;

/// <summary>
/// Handles <see cref="GetBlockchainDataByNameQuery"/> by retrieving paginated blockchain data
/// filtered by name from the repository.
/// </summary>
public class GetBlockchainDataByNameQueryHandler : IRequestHandler<GetBlockchainDataByNameQuery, PaginatedResult<BlockchainDataDto>>
{
	private readonly IBlockchainDataRepository _repository;
	private readonly ILogger<GetBlockchainDataByNameQueryHandler> _logger;

	public GetBlockchainDataByNameQueryHandler(IBlockchainDataRepository repository, ILogger<GetBlockchainDataByNameQueryHandler> logger)
	{
		_repository = repository;
		_logger = logger;
	}

	public async Task<PaginatedResult<BlockchainDataDto>> Handle(GetBlockchainDataByNameQuery request, CancellationToken cancellationToken)
	{
		var page = Math.Max(1, request.Page);
		var pageSize = Math.Clamp(request.PageSize, 1, ApiConstants.Pagination.MaxPageSize);

		_logger.LogInformation("Querying blockchain data for {Name} (page {Page}, pageSize {PageSize})", request.Name, page, pageSize);
		var (items, totalCount) = await _repository.GetByBlockchainNameAsync(request.Name, page, pageSize, cancellationToken);
		_logger.LogInformation("Retrieved {Count} of {Total} records for {Name}", items.Count(), totalCount, request.Name);

		return new PaginatedResult<BlockchainDataDto>
		{
			Items = items.ToDtoList(),
			Page = page,
			PageSize = pageSize,
			TotalCount = totalCount
		};
	}
}
