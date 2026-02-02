using ICMarket.Application.Behaviors;
using ICMarket.Application.DTOs;
using ICMarket.Application.Queries.GetBlockchainDataByName;
using Microsoft.Extensions.Logging;
using Moq;

namespace ICMarket.UnitTests.Behaviors;

[TestFixture]
public class LoggingBehaviorTests
{
	private Mock<ILogger<LoggingBehavior<GetBlockchainDataByNameQuery, PaginatedResult<BlockchainDataDto>>>> _loggerMock = null!;
	private LoggingBehavior<GetBlockchainDataByNameQuery, PaginatedResult<BlockchainDataDto>> _behavior = null!;

	[SetUp]
	public void SetUp()
	{
		_loggerMock = new Mock<ILogger<LoggingBehavior<GetBlockchainDataByNameQuery, PaginatedResult<BlockchainDataDto>>>>();
		_behavior = new LoggingBehavior<GetBlockchainDataByNameQuery, PaginatedResult<BlockchainDataDto>>(_loggerMock.Object);
	}

	[Test]
	public async Task Handle_SuccessfulRequest_ShouldCallNextAndReturnResponse()
	{
		var expectedResult = new PaginatedResult<BlockchainDataDto>
		{
			Items = new List<BlockchainDataDto> { new() { Name = "BTC.main" } },
			Page = 1,
			PageSize = 50,
			TotalCount = 1
		};
		var nextCalled = false;

		var result = await _behavior.Handle(
			new GetBlockchainDataByNameQuery("btc/main"),
			() =>
			{
				nextCalled = true;
				return Task.FromResult(expectedResult);
			},
			CancellationToken.None);

		Assert.That(nextCalled, Is.True);
		Assert.That(result, Is.EqualTo(expectedResult));
	}

	[Test]
	public async Task Handle_SuccessfulRequest_ShouldLogStartAndCompletion()
	{
		var expectedResult = new PaginatedResult<BlockchainDataDto>();

		await _behavior.Handle(
			new GetBlockchainDataByNameQuery("btc/main"),
			() => Task.FromResult(expectedResult),
			CancellationToken.None);

		// Verify logging was called (Information level for start and completion)
		_loggerMock.Verify(
			x => x.Log(
				LogLevel.Information,
				It.IsAny<EventId>(),
				It.IsAny<It.IsAnyType>(),
				It.IsAny<Exception?>(),
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
			Times.Exactly(2)); // Start + Completion
	}

	[Test]
	public void Handle_ExceptionThrown_ShouldLogErrorAndRethrow()
	{
		var expectedException = new InvalidOperationException("Test exception");

		var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
			await _behavior.Handle(
				new GetBlockchainDataByNameQuery("btc/main"),
				() => throw expectedException,
				CancellationToken.None));

		Assert.That(ex, Is.EqualTo(expectedException));

		// Verify error was logged
		_loggerMock.Verify(
			x => x.Log(
				LogLevel.Error,
				It.IsAny<EventId>(),
				It.IsAny<It.IsAnyType>(),
				expectedException,
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
			Times.Once);
	}

	[Test]
	public async Task Handle_ExceptionThrown_ShouldLogStartBeforeError()
	{
		var logCalls = new List<LogLevel>();

		_loggerMock
			.Setup(x => x.Log(
				It.IsAny<LogLevel>(),
				It.IsAny<EventId>(),
				It.IsAny<It.IsAnyType>(),
				It.IsAny<Exception?>(),
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
			.Callback<LogLevel, EventId, object, Exception?, Delegate>((level, _, _, _, _) => logCalls.Add(level));

		try
		{
			await _behavior.Handle(
				new GetBlockchainDataByNameQuery("btc/main"),
				() => throw new Exception("Test"),
				CancellationToken.None);
		}
		catch
		{
			// Expected
		}

		Assert.That(logCalls.Count, Is.EqualTo(2));
		Assert.That(logCalls[0], Is.EqualTo(LogLevel.Information)); // Start
		Assert.That(logCalls[1], Is.EqualTo(LogLevel.Error)); // Error
	}

	[Test]
	public async Task Handle_SlowRequest_ShouldLogElapsedTime()
	{
		await _behavior.Handle(
			new GetBlockchainDataByNameQuery("btc/main"),
			async () =>
			{
				await Task.Delay(50); // Simulate slow operation
				return new PaginatedResult<BlockchainDataDto>();
			},
			CancellationToken.None);

		// Verify completion was logged (includes elapsed time)
		_loggerMock.Verify(
			x => x.Log(
				LogLevel.Information,
				It.IsAny<EventId>(),
				It.IsAny<It.IsAnyType>(),
				It.IsAny<Exception?>(),
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
			Times.Exactly(2));
	}
}
