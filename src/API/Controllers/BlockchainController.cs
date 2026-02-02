using ICMarket.Application.Commands.FetchAndStoreBlockchainData;
using ICMarket.Application.DTOs;
using ICMarket.Application.Queries.GetAllBlockchainData;
using ICMarket.Application.Queries.GetBlockchainDataByName;
using ICMarket.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ICMarket.API.Controllers;

/// <summary>
/// API controller for fetching, storing, and querying blockchain network data
/// from the BlockCypher API (ETH, DASH, BTC, LTC).
/// </summary>
[ApiController]
[Route(ApiConstants.Routes.Blockchain)]
[EnableRateLimiting(ApiConstants.RateLimiting.DefaultPolicy)]
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
	[EnableRateLimiting(ApiConstants.RateLimiting.StrictPolicy)]
	[ProducesResponseType(typeof(IEnumerable<BlockchainDataDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> FetchBlockchainData(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Fetching and storing blockchain data from all endpoints");
		var result = await _mediator.Send(new FetchAndStoreBlockchainDataCommand(), cancellationToken);
		_logger.LogInformation("Successfully fetched and stored {Count} blockchain records", result.Count());
		return Ok(result);
	}

	/// <summary>
	/// Retrieves paginated blockchain data history, ordered by CreatedAt descending.
	/// </summary>
	/// <param name="page">Page number (1-based, default: 1).</param>
	/// <param name="pageSize">Records per page (default: 50, max: 200).</param>
	/// <returns>Paginated blockchain data records with metadata.</returns>
	[HttpGet]
	[ProducesResponseType(typeof(PaginatedResult<BlockchainDataDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Retrieving blockchain data history (page {Page}, pageSize {PageSize})", page, pageSize);
		var result = await _mediator.Send(new GetAllBlockchainDataQuery(page, pageSize), cancellationToken);
		_logger.LogInformation("Retrieved {Count} of {Total} blockchain records", result.Items.Count(), result.TotalCount);
		return Ok(result);
	}

	/// <summary>
	/// Retrieves paginated blockchain data history filtered by blockchain name.
	/// </summary>
	/// <param name="name">Blockchain name (e.g., "BTC.main", "ETH.main"). Case-insensitive.</param>
	/// <param name="page">Page number (1-based, default: 1).</param>
	/// <param name="pageSize">Records per page (default: 50, max: 200).</param>
	/// <returns>Paginated blockchain data records with metadata.</returns>
	[HttpGet("{name}")]
	[ProducesResponseType(typeof(PaginatedResult<BlockchainDataDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> GetByName(string name, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Retrieving blockchain data for {Name} (page {Page}, pageSize {PageSize})", name, page, pageSize);
		var result = await _mediator.Send(new GetBlockchainDataByNameQuery(name, page, pageSize), cancellationToken);
		_logger.LogInformation("Retrieved {Count} of {Total} records for {Name}", result.Items.Count(), result.TotalCount, name);
		return Ok(result);
	}
}
