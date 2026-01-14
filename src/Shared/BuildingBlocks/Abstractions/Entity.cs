namespace BuildingBlocks.Abstractions;

/// <summary>
/// Base class for all domain entities.
/// Provides identity (MongoDB ObjectId format) and common audit fields.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Unique identifier. Uses MongoDB ObjectId format (24 hex characters).
    /// If not set, auto-generates on creation.
    /// </summary>
    public string Id { get; protected set; } = GenerateObjectId();
    
    /// <summary>
    /// When the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the entity was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; protected set; }

    protected Entity() { }

    protected Entity(string id)
    {
        Id = id;
    }

    /// <summary>
    /// Marks the entity as updated.
    /// </summary>
    protected void MarkUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Generates a MongoDB-compatible ObjectId.
    /// Format: 8 hex (timestamp) + 6 hex (machine) + 4 hex (process) + 6 hex (counter)
    /// </summary>
    private static string GenerateObjectId()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var random = new Random();
        var machine = random.Next(0, 0xFFFFFF);
        var process = random.Next(0, 0xFFFF);
        var counter = random.Next(0, 0xFFFFFF);

        return $"{timestamp:x8}{machine:x6}{process:x4}{counter:x6}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (string.IsNullOrEmpty(Id) || string.IsNullOrEmpty(other.Id))
            return false;

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return (GetType().ToString() + Id).GetHashCode();
    }

    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }
}
