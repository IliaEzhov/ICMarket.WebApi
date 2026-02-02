using ICMarket.Application.DTOs;
using ICMarket.Application.Interfaces;
using MediatR;

namespace ICMarket.Application.Queries.GetAllBlockchainData;

/// <summary>
/// CQRS query that retrieves paginated blockchain data history, ordered by CreatedAt descending.
/// </summary>
/// <param name="Page">Page number (1-based). Defaults to 1.</param>
/// <param name="PageSize">Number of records per page. Defaults to 50, max 200.</param>
public record GetAllBlockchainDataQuery(int Page = 1, int PageSize = 50) : IRequest<PaginatedResult<BlockchainDataDto>>, ICacheableQuery
{
	public string CacheKey => $"blockchain-all-p{Page}-s{PageSize}";
	public TimeSpan CacheDuration => TimeSpan.FromMinutes(5);
}
