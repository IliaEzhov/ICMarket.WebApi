namespace ICMarket.Application.DTOs;

/// <summary>
/// Generic wrapper for paginated query results containing items and pagination metadata.
/// </summary>
public class PaginatedResult<T>
{
	public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
	public int Page { get; set; }
	public int PageSize { get; set; }
	public int TotalCount { get; set; }
	public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
