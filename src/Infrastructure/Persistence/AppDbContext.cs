using ICMarket.Domain.Entities;
using ICMarket.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ICMarket.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core database context implementing <see cref="IUnitOfWork"/>.
/// Manages persistence for blockchain data using SQLite.
/// </summary>
public class AppDbContext : DbContext, IUnitOfWork
{
	public DbSet<BlockchainData> BlockchainData => Set<BlockchainData>();

	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
		base.OnModelCreating(modelBuilder);
	}
}
