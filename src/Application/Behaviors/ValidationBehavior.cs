using FluentValidation;
using MediatR;

namespace ICMarket.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior that automatically validates incoming requests using
/// all registered FluentValidation validators. Throws <see cref="ValidationException"/>
/// if any validation failures occur, preventing the request from reaching its handler.
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;

	public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
	{
		_validators = validators;
	}

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		if (!_validators.Any())
			return await next();

		var context = new ValidationContext<TRequest>(request);

		var validationResults = await Task.WhenAll(
			_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

		var failures = validationResults
			.SelectMany(r => r.Errors)
			.Where(f => f is not null)
			.ToList();

		if (failures.Count != 0)
			throw new ValidationException(failures);

		return await next();
	}
}
