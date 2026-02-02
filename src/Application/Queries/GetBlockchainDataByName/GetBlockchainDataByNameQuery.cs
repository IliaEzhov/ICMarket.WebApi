using ICMarket.Application.DTOs;
using ICMarket.Application.Interfaces;
using MediatR;

namespace ICMarket.Application.Queries.GetBlockchainDataByName;

/// <summary>
/// CQRS query that retrieves paginated blockchain data history filtered by blockchain name.
/// </summary>
/// <param name="Name">Blockchain name to filter by (e.g., "BTC.main", "ETH.main"). Case-insensitive.</param>
/// <param name="Page">Page number (1-based). Defaults to 1.</param>
/// <param name="PageSize">Number of records per page. Defaults to 50, max 200.</param>
public record GetBlockchainDataByNameQuery(string Name, int Page = 1, int PageSize = 50) : IRequest<PaginatedResult<BlockchainDataDto>>, ICacheableQuery
{
	public string CacheKey => $"blockchain-{Name.ToLowerInvariant()}-p{Page}-s{PageSize}";
	public TimeSpan CacheDuration => TimeSpan.FromMinutes(5);
}
