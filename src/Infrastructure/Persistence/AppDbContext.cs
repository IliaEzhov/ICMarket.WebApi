using ICMarket.Domain.Entities;
using ICMarket.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ICMarket.Infrastructure.Persistence;

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
