using ICMarket.Application.Interfaces;
using ICMarket.Infrastructure.Configuration;
using ICMarket.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ICMarket.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
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
