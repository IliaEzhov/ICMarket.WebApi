using ICMarket.Domain.Entities;
using ICMarket.Domain.Interfaces;
using ICMarket.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ICMarket.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IBlockchainDataRepository"/>.
/// Provides read/write access to blockchain data with case-insensitive name filtering.
/// </summary>
public class BlockchainDataRepository : IBlockchainDataRepository
{
	private readonly AppDbContext _context;

	public BlockchainDataRepository(AppDbContext context)
	{
		_context = context;
	}

	public async Task<(IEnumerable<BlockchainData> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
	{
		var query = _context.BlockchainData
			.OrderByDescending(x => x.CreatedAt)
			.AsNoTracking();

		var totalCount = await query.CountAsync(cancellationToken);
		var items = await query
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);

		return (items, totalCount);
	}

	public async Task<(IEnumerable<BlockchainData> Items, int TotalCount)> GetByBlockchainNameAsync(string name, int page, int pageSize, CancellationToken cancellationToken = default)
	{
		var query = _context.BlockchainData
			.Where(x => EF.Functions.Collate(x.Name, "NOCASE") == name)
			.OrderByDescending(x => x.CreatedAt)
			.AsNoTracking();

		var totalCount = await query.CountAsync(cancellationToken);
		var items = await query
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);

		return (items, totalCount);
	}

	public async Task AddAsync(BlockchainData entity, CancellationToken cancellationToken = default)
	{
		await _context.BlockchainData.AddAsync(entity, cancellationToken);
	}

	public async Task AddRangeAsync(IEnumerable<BlockchainData> entities, CancellationToken cancellationToken = default)
	{
		await _context.BlockchainData.AddRangeAsync(entities, cancellationToken);
	}
}
