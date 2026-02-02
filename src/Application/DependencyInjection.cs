using System.Reflection;
using FluentValidation;
using ICMarket.Application.Behaviors;
using ICMarket.Application.Interfaces;
using ICMarket.Application.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ICMarket.Application;

/// <summary>
/// Dependency injection extensions for the Application layer.
/// </summary>
public static class DependencyInjection
{
	/// <summary>
	/// Registers Application layer services: MediatR handlers, FluentValidation validators,
	/// and the validation pipeline behavior.
	/// </summary>
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		var assembly = Assembly.GetExecutingAssembly();

		services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
		services.AddValidatorsFromAssembly(assembly);

		// Register pipeline behaviors (order matters: logging → validation → caching → handler)
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

		services.AddSingleton<ICacheInvalidator, CacheInvalidator>();

		return services;
	}
}
