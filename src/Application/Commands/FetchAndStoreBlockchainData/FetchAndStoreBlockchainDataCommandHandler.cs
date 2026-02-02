using ICMarket.Application.DTOs;
using ICMarket.Application.Exceptions;
using ICMarket.Application.Interfaces;
using ICMarket.Application.Mappings;
using ICMarket.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ICMarket.Application.Commands.FetchAndStoreBlockchainData;

/// <summary>
/// Handles <see cref="FetchAndStoreBlockchainDataCommand"/> by fetching data from all
/// BlockCypher endpoints in parallel, persisting it via the repository, and returning DTOs.
/// </summary>
public class FetchAndStoreBlockchainDataCommandHandler : IRequestHandler<FetchAndStoreBlockchainDataCommand, IEnumerable<BlockchainDataDto>>
{
	private readonly IBlockchainService _blockchainService;
	private readonly IBlockchainDataRepository _repository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ICacheInvalidator _cacheInvalidator;
	private readonly ILogger<FetchAndStoreBlockchainDataCommandHandler> _logger;

	public FetchAndStoreBlockchainDataCommandHandler(
		IBlockchainService blockchainService,
		IBlockchainDataRepository repository,
		IUnitOfWork unitOfWork,
		ICacheInvalidator cacheInvalidator,
		ILogger<FetchAndStoreBlockchainDataCommandHandler> logger)
	{
		_blockchainService = blockchainService;
		_repository = repository;
		_unitOfWork = unitOfWork;
		_cacheInvalidator = cacheInvalidator;
		_logger = logger;
	}

	public async Task<IEnumerable<BlockchainDataDto>> Handle(FetchAndStoreBlockchainDataCommand request, CancellationToken cancellationToken)
	{
		try
		{
			_logger.LogInformation("Fetching blockchain data from all configured endpoints");
			var blockchainData = await _blockchainService.FetchAllBlockchainDataAsync(cancellationToken);

			var dataList = blockchainData.ToList();
			_logger.LogInformation("Fetched {Count} blockchain records, persisting to database", dataList.Count);

			await _repository.AddRangeAsync(dataList, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);

			_cacheInvalidator.InvalidateAll();
			_logger.LogInformation("Successfully persisted {Count} blockchain records and invalidated cache", dataList.Count);
			return dataList.ToDtoList();
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError(ex, "External API call failed while fetching blockchain data");
			throw new ExternalServiceException("BlockCypher", "Failed to fetch blockchain data from BlockCypher API.", ex);
		}
		catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
		{
			_logger.LogError(ex, "External API call timed out while fetching blockchain data");
			throw new ExternalServiceException("BlockCypher", "BlockCypher API request timed out.", ex);
		}
	}
}
