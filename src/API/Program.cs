using System.Text.Json.Serialization;
using ICMarket.API.Filters;
using ICMarket.Application;
using ICMarket.Infrastructure;
using ICMarket.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add controllers with JSON serialization options
builder.Services.AddControllers(options =>
	{
		options.Filters.Add<ValidationExceptionFilter>();
	})
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
		options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
	});

// Register Application and Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
	{
		Title = "ICMarket Blockchain API",
		Version = "v1",
		Description = "Web API for storing and retrieving blockchain data from BlockCypher"
	});
});

// CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
	{
		policy.AllowAnyOrigin()
			.AllowAnyMethod()
			.AllowAnyHeader();
	});
});

// Health Checks
builder.Services.AddHealthChecks()
	.AddSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
		?? throw new InvalidOperationException("Missing 'DefaultConnection' connection string"));

var app = builder.Build();

// Auto-create database on startup
using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	await dbContext.Database.EnsureCreatedAsync();
}

// Swagger UI enabled unconditionally
app.UseSwagger();
app.UseSwaggerUI(options =>
{
	options.SwaggerEndpoint("/swagger/v1/swagger.json", "ICMarket Blockchain API v1");
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.MapControllers();
app.MapHealthChecks("/health");

await app.RunAsync();
