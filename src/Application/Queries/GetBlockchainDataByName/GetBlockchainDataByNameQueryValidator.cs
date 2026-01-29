using FluentValidation;

namespace ICMarket.Application.Queries.GetBlockchainDataByName;

public class GetBlockchainDataByNameQueryValidator : AbstractValidator<GetBlockchainDataByNameQuery>
{
	private static readonly string[] ValidBlockchainNames = { "ETH.main", "DASH.main", "BTC.main", "BTC.test3", "LTC.main" };

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
