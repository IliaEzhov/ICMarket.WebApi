using ICMarket.Application.DTOs;
using ICMarket.Application.Interfaces;
using ICMarket.Application.Mappings;
using ICMarket.Domain.Interfaces;
using MediatR;

namespace ICMarket.Application.Commands.FetchAndStoreBlockchainData;

public class FetchAndStoreBlockchainDataCommandHandler : IRequestHandler<FetchAndStoreBlockchainDataCommand, IEnumerable<BlockchainDataDto>>
{
	private readonly IBlockchainService _blockchainService;
	private readonly IBlockchainDataRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public FetchAndStoreBlockchainDataCommandHandler(
		IBlockchainService blockchainService,
		IBlockchainDataRepository repository,
		IUnitOfWork unitOfWork)
	{
		_blockchainService = blockchainService;
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<IEnumerable<BlockchainDataDto>> Handle(FetchAndStoreBlockchainDataCommand request, CancellationToken cancellationToken)
	{
		var blockchainData = await _blockchainService.FetchAllBlockchainDataAsync(cancellationToken);

		var dataList = blockchainData.ToList();

		await _repository.AddRangeAsync(dataList, cancellationToken);
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return dataList.ToDtoList();
	}
}
