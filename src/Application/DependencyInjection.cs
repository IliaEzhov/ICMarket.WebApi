using System.Reflection;
using FluentValidation;
using ICMarket.Application.Behaviors;
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
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

		return services;
	}
}
