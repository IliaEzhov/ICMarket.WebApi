namespace ICMarket.Domain.Entities;

/// <summary>
/// Base entity providing common properties for all domain entities.
/// </summary>
public abstract class BaseEntity
{
	/// <summary>
	/// Unique identifier for the entity.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Timestamp indicating when the entity was created (UTC).
	/// </summary>
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
