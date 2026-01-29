using ICMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMarket.Infrastructure.Persistence.Configurations;

public class BlockchainDataConfiguration : IEntityTypeConfiguration<BlockchainData>
{
	public void Configure(EntityTypeBuilder<BlockchainData> builder)
	{
		builder.ToTable("BlockchainData");

		builder.HasKey(e => e.Id);

		builder.Property(e => e.Id)
			.ValueGeneratedNever();

		builder.Property(e => e.CreatedAt)
			.IsRequired();

		builder.Property(e => e.Name)
			.IsRequired()
			.HasMaxLength(50);

		builder.Property(e => e.Hash)
			.IsRequired()
			.HasMaxLength(256);

		builder.Property(e => e.Time)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(e => e.LatestUrl)
			.HasMaxLength(500);

		builder.Property(e => e.PreviousHash)
			.HasMaxLength(256);

		builder.Property(e => e.PreviousUrl)
			.HasMaxLength(500);

		builder.Property(e => e.LastForkHash)
			.HasMaxLength(256);

		builder.HasIndex(e => new { e.Name, e.CreatedAt })
			.HasDatabaseName("IX_BlockchainData_Name_CreatedAt");
		builder.HasIndex(e => e.CreatedAt);
	}
}
