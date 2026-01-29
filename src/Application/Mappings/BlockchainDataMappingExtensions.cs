using ICMarket.Application.DTOs;
using ICMarket.Domain.Entities;

namespace ICMarket.Application.Mappings;

public static class BlockchainDataMappingExtensions
{
	public static BlockchainDataDto ToDto(this BlockchainData entity)
	{
		return new BlockchainDataDto
		{
			Id = entity.Id,
			CreatedAt = entity.CreatedAt,
			Name = entity.Name,
			Height = entity.Height,
			Hash = entity.Hash,
			Time = entity.Time,
			LatestUrl = entity.LatestUrl,
			PreviousHash = entity.PreviousHash,
			PreviousUrl = entity.PreviousUrl,
			PeerCount = entity.PeerCount,
			UnconfirmedCount = entity.UnconfirmedCount,
			LastForkHeight = entity.LastForkHeight,
			LastForkHash = entity.LastForkHash,
			HighFeePerKb = entity.HighFeePerKb,
			MediumFeePerKb = entity.MediumFeePerKb,
			LowFeePerKb = entity.LowFeePerKb,
			HighGasPrice = entity.HighGasPrice,
			MediumGasPrice = entity.MediumGasPrice,
			LowGasPrice = entity.LowGasPrice,
			HighPriorityFee = entity.HighPriorityFee,
			MediumPriorityFee = entity.MediumPriorityFee,
			LowPriorityFee = entity.LowPriorityFee,
			BaseFee = entity.BaseFee,
		};
	}

	public static IEnumerable<BlockchainDataDto> ToDtoList(this IEnumerable<BlockchainData> entities)
	{
		return entities.Select(e => e.ToDto());
	}
}
