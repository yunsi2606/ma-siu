namespace BuildingBlocks.Abstractions;

/// <summary>
/// Base class for all domain entities.
/// Provides identity and equality based on Id.
/// </summary>
public abstract class Entity
{
    public string Id { get; protected set; } = null!;
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }

    protected Entity() { }

    protected Entity(string id)
    {
        Id = id;
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
