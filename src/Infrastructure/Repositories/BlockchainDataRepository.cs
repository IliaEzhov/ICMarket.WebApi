using ICMarket.Domain.Entities;
using ICMarket.Domain.Interfaces;
using ICMarket.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ICMarket.Infrastructure.Repositories;

public class BlockchainDataRepository : IBlockchainDataRepository
{
	private readonly AppDbContext _context;

	public BlockchainDataRepository(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<BlockchainData>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.BlockchainData
			.OrderByDescending(x => x.CreatedAt)
			.AsNoTracking()
			.ToListAsync(cancellationToken);
	}

	public async Task<IEnumerable<BlockchainData>> GetByBlockchainNameAsync(string name, CancellationToken cancellationToken = default)
	{
		return await _context.BlockchainData
			.Where(x => EF.Functions.Collate(x.Name, "NOCASE") == name)
			.OrderByDescending(x => x.CreatedAt)
			.AsNoTracking()
			.ToListAsync(cancellationToken);
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
