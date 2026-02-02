using ICMarket.Domain.Entities;
using ICMarket.Infrastructure.Models;

namespace ICMarket.Infrastructure.Mappings;

/// <summary>
/// Extension methods for mapping <see cref="BlockcypherResponse"/> API models
/// to <see cref="BlockchainData"/> domain entities.
/// </summary>
public static class BlockcypherResponseMappingExtensions
{
	/// <summary>
	/// Maps a <see cref="BlockcypherResponse"/> to a <see cref="BlockchainData"/> domain entity.
	/// </summary>
	public static BlockchainData ToEntity(this BlockcypherResponse response)
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
