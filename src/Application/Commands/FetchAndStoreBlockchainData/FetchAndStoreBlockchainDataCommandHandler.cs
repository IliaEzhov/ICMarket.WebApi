using ICMarket.Application.DTOs;
using ICMarket.Application.Interfaces;
using ICMarket.Application.Mappings;
using ICMarket.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ICMarket.Application.Commands.FetchAndStoreBlockchainData;

public class FetchAndStoreBlockchainDataCommandHandler : IRequestHandler<FetchAndStoreBlockchainDataCommand, IEnumerable<BlockchainDataDto>>
{
	private readonly IBlockchainService _blockchainService;
	private readonly IBlockchainDataRepository _repository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ILogger<FetchAndStoreBlockchainDataCommandHandler> _logger;

	public FetchAndStoreBlockchainDataCommandHandler(
		IBlockchainService blockchainService,
		IBlockchainDataRepository repository,
		IUnitOfWork unitOfWork,
		ILogger<FetchAndStoreBlockchainDataCommandHandler> logger)
	{
		_blockchainService = blockchainService;
		_repository = repository;
		_unitOfWork = unitOfWork;
		_logger = logger;
	}

	public async Task<IEnumerable<BlockchainDataDto>> Handle(FetchAndStoreBlockchainDataCommand request, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Fetching blockchain data from all configured endpoints");
		var blockchainData = await _blockchainService.FetchAllBlockchainDataAsync(cancellationToken);

		var dataList = blockchainData.ToList();
		_logger.LogInformation("Fetched {Count} blockchain records, persisting to database", dataList.Count);

		await _repository.AddRangeAsync(dataList, cancellationToken);
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		_logger.LogInformation("Successfully persisted {Count} blockchain records", dataList.Count);
		return dataList.ToDtoList();
	}
}
