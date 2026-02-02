using FluentValidation;
using FluentValidation.Results;
using ICMarket.Application.Behaviors;
using ICMarket.Application.DTOs;
using ICMarket.Application.Queries.GetBlockchainDataByName;
using MediatR;
using Moq;

namespace ICMarket.UnitTests.Behaviors;

[TestFixture]
public class ValidationBehaviorTests
{
	[Test]
	public async Task Handle_NoValidators_ShouldCallNext()
	{
		var validators = Enumerable.Empty<IValidator<GetBlockchainDataByNameQuery>>();
		var behavior = new ValidationBehavior<GetBlockchainDataByNameQuery, IEnumerable<BlockchainDataDto>>(validators);

		var expectedResult = new List<BlockchainDataDto> { new() { Name = "BTC.main" } };
		var nextCalled = false;

		var result = await behavior.Handle(
			new GetBlockchainDataByNameQuery("btc/main"),
			() =>
			{
				nextCalled = true;
				return Task.FromResult<IEnumerable<BlockchainDataDto>>(expectedResult);
			},
			CancellationToken.None);

		Assert.That(nextCalled, Is.True);
		Assert.That(result, Is.EqualTo(expectedResult));
	}

	[Test]
	public async Task Handle_ValidRequest_ShouldCallNext()
	{
		var validatorMock = new Mock<IValidator<GetBlockchainDataByNameQuery>>();
		validatorMock
			.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<GetBlockchainDataByNameQuery>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new ValidationResult());

		var validators = new[] { validatorMock.Object };
		var behavior = new ValidationBehavior<GetBlockchainDataByNameQuery, IEnumerable<BlockchainDataDto>>(validators);

		var expectedResult = new List<BlockchainDataDto> { new() { Name = "BTC.main" } };
		var nextCalled = false;

		var result = await behavior.Handle(
			new GetBlockchainDataByNameQuery("btc/main"),
			() =>
			{
				nextCalled = true;
				return Task.FromResult<IEnumerable<BlockchainDataDto>>(expectedResult);
			},
			CancellationToken.None);

		Assert.That(nextCalled, Is.True);
		Assert.That(result, Is.EqualTo(expectedResult));
	}

	[Test]
	public void Handle_InvalidRequest_ShouldThrowValidationException()
	{
		var failures = new List<ValidationFailure>
		{
			new("Name", "Blockchain name is required.")
		};

		var validatorMock = new Mock<IValidator<GetBlockchainDataByNameQuery>>();
		validatorMock
			.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<GetBlockchainDataByNameQuery>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new ValidationResult(failures));

		var validators = new[] { validatorMock.Object };
		var behavior = new ValidationBehavior<GetBlockchainDataByNameQuery, IEnumerable<BlockchainDataDto>>(validators);

		Assert.ThrowsAsync<ValidationException>(async () =>
			await behavior.Handle(
				new GetBlockchainDataByNameQuery(string.Empty),
				() => Task.FromResult<IEnumerable<BlockchainDataDto>>(new List<BlockchainDataDto>()),
				CancellationToken.None));
	}

	[Test]
	public void Handle_InvalidRequest_ShouldNotCallNext()
	{
		var failures = new List<ValidationFailure>
		{
			new("Name", "Blockchain name is required.")
		};

		var validatorMock = new Mock<IValidator<GetBlockchainDataByNameQuery>>();
		validatorMock
			.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<GetBlockchainDataByNameQuery>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new ValidationResult(failures));

		var validators = new[] { validatorMock.Object };
		var behavior = new ValidationBehavior<GetBlockchainDataByNameQuery, IEnumerable<BlockchainDataDto>>(validators);

		var nextCalled = false;

		Assert.ThrowsAsync<ValidationException>(async () =>
			await behavior.Handle(
				new GetBlockchainDataByNameQuery(string.Empty),
				() =>
				{
					nextCalled = true;
					return Task.FromResult<IEnumerable<BlockchainDataDto>>(new List<BlockchainDataDto>());
				},
				CancellationToken.None));

		Assert.That(nextCalled, Is.False);
	}
}
