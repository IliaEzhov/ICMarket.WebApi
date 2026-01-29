using FluentValidation.TestHelper;
using ICMarket.Application.Queries.GetBlockchainDataByName;

namespace ICMarket.UnitTests.Validators;

[TestFixture]
public class GetBlockchainDataByNameQueryValidatorTests
{
	private GetBlockchainDataByNameQueryValidator _validator = null!;

	[SetUp]
	public void SetUp()
	{
		_validator = new GetBlockchainDataByNameQueryValidator();
	}

	[TestCase("eth/main")]
	[TestCase("dash/main")]
	[TestCase("btc/main")]
	[TestCase("btc/test3")]
	[TestCase("ltc/main")]
	public void Validate_ValidBlockchainName_ShouldNotHaveErrors(string name)
	{
		var query = new GetBlockchainDataByNameQuery(name);

		var result = _validator.TestValidate(query);

		result.ShouldNotHaveAnyValidationErrors();
	}

	[TestCase("ETH/MAIN")]
	[TestCase("Btc/Main")]
	public void Validate_CaseInsensitiveName_ShouldNotHaveErrors(string name)
	{
		var query = new GetBlockchainDataByNameQuery(name);

		var result = _validator.TestValidate(query);

		result.ShouldNotHaveAnyValidationErrors();
	}

	[Test]
	public void Validate_EmptyName_ShouldHaveError()
	{
		var query = new GetBlockchainDataByNameQuery(string.Empty);

		var result = _validator.TestValidate(query);

		result.ShouldHaveValidationErrorFor(x => x.Name);
	}

	[Test]
	public void Validate_InvalidName_ShouldHaveError()
	{
		var query = new GetBlockchainDataByNameQuery("invalid/chain");

		var result = _validator.TestValidate(query);

		result.ShouldHaveValidationErrorFor(x => x.Name);
	}
}
