using System.Text.Json.Serialization;
using ICMarket.API.Filters;
using ICMarket.Application;
using ICMarket.Common.Constants;
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
	options.SwaggerDoc(SwaggerConstants.ApiInfo.Version, new Microsoft.OpenApi.Models.OpenApiInfo
	{
		Title = SwaggerConstants.ApiInfo.Title,
		Version = SwaggerConstants.ApiInfo.Version,
		Description = SwaggerConstants.ApiInfo.Description
		});
});

// CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy(ApiConstants.Cors.AllowAll, policy =>
	{
		policy.AllowAnyOrigin()
			.AllowAnyMethod()
			.AllowAnyHeader();
	});
});

// Health Checks
builder.Services.AddHealthChecks()
	.AddSqlite(builder.Configuration.GetConnectionString(DatabaseConstants.Configuration.DefaultConnection)
		?? throw new InvalidOperationException($"Missing '{DatabaseConstants.Configuration.DefaultConnection}' connection string"));

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
	options.SwaggerEndpoint(SwaggerConstants.Endpoints.SwaggerJson, $"{SwaggerConstants.ApiInfo.Title} {SwaggerConstants.ApiInfo.Version}");
});

app.UseHttpsRedirection();
app.UseCors(ApiConstants.Cors.AllowAll);
app.MapControllers();
app.MapHealthChecks(ApiConstants.Routes.Health);

await app.RunAsync();
