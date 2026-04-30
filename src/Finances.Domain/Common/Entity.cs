namespace Finances.Domain.Common;

public abstract class Entity<TId>
    where TId : struct
{
    protected Entity(TId id)
    {
        Id = id;
    }

    public TId Id { get; }
}
