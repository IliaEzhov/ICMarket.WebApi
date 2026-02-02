using ICMarket.Application.Interfaces;
using ICMarket.Domain.Interfaces;
using ICMarket.Infrastructure.Configuration;
using ICMarket.Infrastructure.Persistence;
using ICMarket.Infrastructure.Repositories;
using ICMarket.Common.Constants;
using ICMarket.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ICMarket.Infrastructure;

/// <summary>
/// Dependency injection extensions for the Infrastructure layer.
/// </summary>
public static class DependencyInjection
{
	/// <summary>
	/// Registers Infrastructure layer services: EF Core DbContext (SQLite), repository,
	/// Unit of Work, BlockCypher HTTP client, and configuration bindings.
	/// </summary>
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<AppDbContext>(options =>
			options.UseSqlite(configuration.GetConnectionString(DatabaseConstants.Configuration.DefaultConnection)));

		services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>());
		services.AddScoped<IBlockchainDataRepository, BlockchainDataRepository>();

		var blockcypherSettings = new BlockcypherSettings();
		configuration.GetSection(BlockcypherSettings.SectionName).Bind(blockcypherSettings);

		services.Configure<BlockcypherSettings>(configuration.GetSection(BlockcypherSettings.SectionName));

		services.AddHttpClient<IBlockchainService, BlockcypherService>(client =>
		{
			client.BaseAddress = new Uri(blockcypherSettings.BaseUrl);
			client.DefaultRequestHeaders.Add("Accept", "application/json");
		});

		return services;
	}
}
