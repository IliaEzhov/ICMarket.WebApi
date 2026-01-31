using ICMarket.Application.Interfaces;
using ICMarket.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ICMarket.FunctionalTests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
	public Mock<IBlockchainService> BlockchainServiceMock { get; } = new();

	private SqliteConnection? _connection;

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureServices(services =>
		{
			var dbContextDescriptor = services.SingleOrDefault(
				d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
			if (dbContextDescriptor != null)
				services.Remove(dbContextDescriptor);

			var blockchainServiceDescriptors = services
				.Where(d => d.ServiceType == typeof(IBlockchainService))
				.ToList();
			foreach (var descriptor in blockchainServiceDescriptors)
				services.Remove(descriptor);

			_connection = new SqliteConnection("DataSource=:memory:");
			_connection.Open();

			services.AddDbContext<AppDbContext>(options =>
				options.UseSqlite(_connection));

			services.AddScoped<IBlockchainService>(_ => BlockchainServiceMock.Object);

			var sp = services.BuildServiceProvider();
			using var scope = sp.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
			db.Database.EnsureCreated();
		});
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		if (disposing)
		{
			_connection?.Dispose();
		}
	}
}
