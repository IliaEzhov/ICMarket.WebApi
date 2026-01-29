using FluentValidation;
using ICMarket.Common.Constants;

namespace ICMarket.Application.Queries.GetBlockchainDataByName;

public class GetBlockchainDataByNameQueryValidator : AbstractValidator<GetBlockchainDataByNameQuery>
{
	private static readonly string[] ValidBlockchainNames = BlockchainConstants.Names.All;

	public GetBlockchainDataByNameQueryValidator()
	{
		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage("Blockchain name is required.");

		RuleFor(x => x.Name)
			.Must(name => ValidBlockchainNames.Contains(name, StringComparer.OrdinalIgnoreCase))
			.When(x => !string.IsNullOrEmpty(x.Name))
			.WithMessage($"Blockchain name must be one of: {string.Join(", ", ValidBlockchainNames)}.");
	}
}
