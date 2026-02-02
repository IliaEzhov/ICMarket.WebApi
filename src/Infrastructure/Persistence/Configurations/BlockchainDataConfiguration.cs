using ICMarket.Common.Constants;
using ICMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMarket.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core entity configuration for <see cref="BlockchainData"/>.
/// Defines table name, column max lengths, indexes, and required fields.
/// </summary>
public class BlockchainDataConfiguration : IEntityTypeConfiguration<BlockchainData>
{
	public void Configure(EntityTypeBuilder<BlockchainData> builder)
	{
		builder.ToTable(DatabaseConstants.Tables.BlockchainData);

		builder.HasKey(e => e.Id);

		builder.Property(e => e.Id)
			.ValueGeneratedNever();

		builder.Property(e => e.CreatedAt)
			.IsRequired();

		builder.Property(e => e.Name)
			.IsRequired()
			.HasMaxLength(DatabaseConstants.ColumnLengths.Name);

		builder.Property(e => e.Hash)
			.IsRequired()
			.HasMaxLength(DatabaseConstants.ColumnLengths.Hash);

		builder.Property(e => e.Time)
			.IsRequired()
			.HasMaxLength(DatabaseConstants.ColumnLengths.Time);

		builder.Property(e => e.LatestUrl)
			.HasMaxLength(DatabaseConstants.ColumnLengths.LatestUrl);

		builder.Property(e => e.PreviousHash)
			.HasMaxLength(DatabaseConstants.ColumnLengths.PreviousHash);

		builder.Property(e => e.PreviousUrl)
			.HasMaxLength(DatabaseConstants.ColumnLengths.PreviousUrl);

		builder.Property(e => e.LastForkHash)
			.HasMaxLength(DatabaseConstants.ColumnLengths.LastForkHash);

		builder.HasIndex(e => new { e.Name, e.CreatedAt })
			.HasDatabaseName(DatabaseConstants.Indexes.BlockchainDataNameCreatedAt);
		builder.HasIndex(e => e.CreatedAt);
	}
}
