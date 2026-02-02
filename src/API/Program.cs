using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using ICMarket.API.Filters;
using Microsoft.AspNetCore.RateLimiting;
using ICMarket.API.Middleware;
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

	var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
	if (File.Exists(xmlPath))
		options.IncludeXmlComments(xmlPath);
});

// CORS (configurable per environment via appsettings)
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["*"];
var corsMethods = builder.Configuration.GetSection("Cors:AllowedMethods").Get<string[]>() ?? ["*"];
var corsHeaders = builder.Configuration.GetSection("Cors:AllowedHeaders").Get<string[]>() ?? ["*"];

builder.Services.AddCors(options =>
{
	options.AddPolicy(ApiConstants.Cors.PolicyName, policy =>
	{
		if (corsOrigins.Contains("*"))
			policy.AllowAnyOrigin();
		else
			policy.WithOrigins(corsOrigins);

		if (corsMethods.Contains("*"))
			policy.AllowAnyMethod();
		else
			policy.WithMethods(corsMethods);

		if (corsHeaders.Contains("*"))
			policy.AllowAnyHeader();
		else
			policy.WithHeaders(corsHeaders);
	});
});

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
	options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

	options.AddFixedWindowLimiter(ApiConstants.RateLimiting.DefaultPolicy, opt =>
	{
		opt.PermitLimit = builder.Configuration.GetValue("RateLimiting:PermitLimit", 100);
		opt.Window = TimeSpan.FromSeconds(builder.Configuration.GetValue("RateLimiting:WindowInSeconds", 60));
		opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
		opt.QueueLimit = 0;
	});

	options.AddFixedWindowLimiter(ApiConstants.RateLimiting.StrictPolicy, opt =>
	{
		opt.PermitLimit = builder.Configuration.GetValue("RateLimiting:FetchPermitLimit", 5);
		opt.Window = TimeSpan.FromSeconds(builder.Configuration.GetValue("RateLimiting:FetchWindowInSeconds", 60));
		opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
		opt.QueueLimit = 0;
	});
});

// In-Memory Cache
builder.Services.AddMemoryCache();

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

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Swagger UI enabled unconditionally
app.UseSwagger();
app.UseSwaggerUI(options =>
{
	options.SwaggerEndpoint(SwaggerConstants.Endpoints.SwaggerJson, $"{SwaggerConstants.ApiInfo.Title} {SwaggerConstants.ApiInfo.Version}");
});

app.UseHttpsRedirection();
app.UseCors(ApiConstants.Cors.PolicyName);
app.UseRateLimiter();
app.MapControllers();
app.MapHealthChecks(ApiConstants.Routes.Health);

await app.RunAsync();

public partial class Program { }
