using ICMarket.Application.Commands.FetchAndStoreBlockchainData;
using ICMarket.Application.DTOs;
using ICMarket.Application.Queries.GetAllBlockchainData;
using ICMarket.Application.Queries.GetBlockchainDataByName;
using ICMarket.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ICMarket.API.Controllers;

[ApiController]
[Route(ApiConstants.Routes.Blockchain)]
public class BlockchainController : ControllerBase
{
	private readonly IMediator _mediator;

	public BlockchainController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpPost("fetch")]
	[ProducesResponseType(typeof(IEnumerable<BlockchainDataDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> FetchBlockchainData(CancellationToken cancellationToken)
	{
		var result = await _mediator.Send(new FetchAndStoreBlockchainDataCommand(), cancellationToken);
		return Ok(result);
	}

	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<BlockchainDataDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
	{
		var result = await _mediator.Send(new GetAllBlockchainDataQuery(), cancellationToken);
		return Ok(result);
	}

	[HttpGet("{name}")]
	[ProducesResponseType(typeof(IEnumerable<BlockchainDataDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> GetByName(string name, CancellationToken cancellationToken)
	{
		var result = await _mediator.Send(new GetBlockchainDataByNameQuery(name), cancellationToken);
		return Ok(result);
	}
}
