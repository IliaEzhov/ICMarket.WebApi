using FluentValidation.TestHelper;
using ICMarket.Application.Queries.GetBlockchainDataByName;
using ICMarket.Common.Constants;

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

	[TestCase(BlockchainConstants.Names.EthMain)]
	[TestCase(BlockchainConstants.Names.DashMain)]
	[TestCase(BlockchainConstants.Names.BtcMain)]
	[TestCase(BlockchainConstants.Names.BtcTest3)]
	[TestCase(BlockchainConstants.Names.LtcMain)]
	public void Validate_ValidBlockchainName_ShouldNotHaveErrors(string name)
	{
		var query = new GetBlockchainDataByNameQuery(name);

		var result = _validator.TestValidate(query);

		result.ShouldNotHaveAnyValidationErrors();
	}

	[TestCase("eth.main")]
	[TestCase("btc.main")]
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
		var query = new GetBlockchainDataByNameQuery("invalid.chain");

		var result = _validator.TestValidate(query);

		result.ShouldHaveValidationErrorFor(x => x.Name);
	}
}
