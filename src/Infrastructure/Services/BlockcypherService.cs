using System.Net.Http.Json;
using ICMarket.Application.Interfaces;
using ICMarket.Domain.Entities;
using ICMarket.Infrastructure.Configuration;
using ICMarket.Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ICMarket.Infrastructure.Services;

public class BlockcypherService : IBlockchainService
{
	private readonly HttpClient _httpClient;
	private readonly BlockcypherSettings _settings;
	private readonly ILogger<BlockcypherService> _logger;

	public BlockcypherService(
		HttpClient httpClient,
		IOptions<BlockcypherSettings> settings,
		ILogger<BlockcypherService> logger)
	{
		_httpClient = httpClient;
		_settings = settings.Value;
		_logger = logger;
	}

	public async Task<IEnumerable<BlockchainData>> FetchAllBlockchainDataAsync(CancellationToken cancellationToken = default)
	{
		var tasks = _settings.Endpoints
			.Select(endpoint => FetchBlockchainDataAsync(endpoint, cancellationToken))
			.ToList();

		var results = await Task.WhenAll(tasks);

		return results.Where(r => r is not null).Cast<BlockchainData>();
	}

	private async Task<BlockchainData?> FetchBlockchainDataAsync(string endpoint, CancellationToken cancellationToken)
	{
		try
		{
			_logger.LogInformation("Fetching blockchain data from {Endpoint}", endpoint);

			var response = await _httpClient.GetFromJsonAsync<BlockcypherResponse>(endpoint, cancellationToken);

			if (response is null)
			{
				_logger.LogWarning("Received null response from {Endpoint}", endpoint);
				return null;
			}

			return MapToEntity(response);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to fetch blockchain data from {Endpoint}", endpoint);
			return null;
		}
	}

	private static BlockchainData MapToEntity(BlockcypherResponse response)
	{
		return new BlockchainData
		{
			Id = Guid.NewGuid(),
			CreatedAt = DateTime.UtcNow,
			Name = response.Name,
			Height = response.Height,
			Hash = response.Hash,
			Time = response.Time,
			LatestUrl = response.LatestUrl,
			PreviousHash = response.PreviousHash,
			PreviousUrl = response.PreviousUrl,
			PeerCount = response.PeerCount,
			UnconfirmedCount = response.UnconfirmedCount,
			LastForkHeight = response.LastForkHeight,
			LastForkHash = response.LastForkHash,
			HighFeePerKb = response.HighFeePerKb,
			MediumFeePerKb = response.MediumFeePerKb,
			LowFeePerKb = response.LowFeePerKb,
			HighGasPrice = response.HighGasPrice,
			MediumGasPrice = response.MediumGasPrice,
			LowGasPrice = response.LowGasPrice,
			HighPriorityFee = response.HighPriorityFee,
			MediumPriorityFee = response.MediumPriorityFee,
			LowPriorityFee = response.LowPriorityFee,
			BaseFee = response.BaseFee
		};
	}
}
