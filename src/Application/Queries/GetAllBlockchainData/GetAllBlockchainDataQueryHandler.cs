using ICMarket.Application.DTOs;
using ICMarket.Application.Mappings;
using ICMarket.Common.Constants;
using ICMarket.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ICMarket.Application.Queries.GetAllBlockchainData;

/// <summary>
/// Handles <see cref="GetAllBlockchainDataQuery"/> by retrieving paginated blockchain data from the repository.
/// </summary>
public class GetAllBlockchainDataQueryHandler : IRequestHandler<GetAllBlockchainDataQuery, PaginatedResult<BlockchainDataDto>>
{
	private readonly IBlockchainDataRepository _repository;
	private readonly ILogger<GetAllBlockchainDataQueryHandler> _logger;

	public GetAllBlockchainDataQueryHandler(IBlockchainDataRepository repository, ILogger<GetAllBlockchainDataQueryHandler> logger)
	{
		_repository = repository;
		_logger = logger;
	}

	public async Task<PaginatedResult<BlockchainDataDto>> Handle(GetAllBlockchainDataQuery request, CancellationToken cancellationToken)
	{
		var page = Math.Max(1, request.Page);
		var pageSize = Math.Clamp(request.PageSize, 1, ApiConstants.Pagination.MaxPageSize);

		_logger.LogInformation("Querying all blockchain data (page {Page}, pageSize {PageSize})", page, pageSize);
		var (items, totalCount) = await _repository.GetAllAsync(page, pageSize, cancellationToken);
		_logger.LogInformation("Retrieved {Count} of {Total} blockchain records", items.Count(), totalCount);

		return new PaginatedResult<BlockchainDataDto>
		{
			Items = items.ToDtoList(),
			Page = page,
			PageSize = pageSize,
			TotalCount = totalCount
		};
	}
}
