using ICMarket.Application.Commands.FetchAndStoreBlockchainData;
using ICMarket.Application.DTOs;
using ICMarket.Application.Queries.GetAllBlockchainData;
using ICMarket.Application.Queries.GetBlockchainDataByName;
using ICMarket.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ICMarket.API.Controllers;

/// <summary>
/// API controller for fetching, storing, and querying blockchain network data
/// from the BlockCypher API (ETH, DASH, BTC, LTC).
/// </summary>
[ApiController]
[Route(ApiConstants.Routes.Blockchain)]
public class BlockchainController : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly ILogger<BlockchainController> _logger;

	public BlockchainController(IMediator mediator, ILogger<BlockchainController> logger)
	{
		_mediator = mediator;
		_logger = logger;
	}

	/// <summary>
	/// Fetches blockchain data from all configured BlockCypher endpoints and stores the results.
	/// </summary>
	/// <returns>The newly fetched and stored blockchain data records.</returns>
	[HttpPost("fetch")]
	[ProducesResponseType(typeof(IEnumerable<BlockchainDataDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> FetchBlockchainData(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Fetching and storing blockchain data from all endpoints");
		var result = await _mediator.Send(new FetchAndStoreBlockchainDataCommand(), cancellationToken);
		_logger.LogInformation("Successfully fetched and stored {Count} blockchain records", result.Count());
		return Ok(result);
	}

	/// <summary>
	/// Retrieves all stored blockchain data history, ordered by CreatedAt descending.
	/// </summary>
	/// <returns>All blockchain data records.</returns>
	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<BlockchainDataDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Retrieving all blockchain data history");
		var result = await _mediator.Send(new GetAllBlockchainDataQuery(), cancellationToken);
		_logger.LogInformation("Retrieved {Count} blockchain records", result.Count());
		return Ok(result);
	}

	/// <summary>
	/// Retrieves stored blockchain data history filtered by blockchain name.
	/// </summary>
	/// <param name="name">Blockchain name (e.g., "BTC.main", "ETH.main"). Case-insensitive.</param>
	/// <returns>Blockchain data records for the specified blockchain.</returns>
	[HttpGet("{name}")]
	[ProducesResponseType(typeof(IEnumerable<BlockchainDataDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> GetByName(string name, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Retrieving blockchain data for {Name}", name);
		var result = await _mediator.Send(new GetBlockchainDataByNameQuery(name), cancellationToken);
		_logger.LogInformation("Retrieved {Count} records for {Name}", result.Count(), name);
		return Ok(result);
	}
}
