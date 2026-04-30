namespace Finances.Domain.Common;

public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : struct
{
    private readonly List<IDomainEvent> domainEvents = [];

    protected AggregateRoot(TId id)
        : base(id)
    {
    }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();

    public void ClearDomainEvents()
    {
        domainEvents.Clear();
    }

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }
}
