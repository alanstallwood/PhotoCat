namespace PhotoCat.Application.Interfaces;

public interface IDomainEventDispatcher
{
    void Raise(IDomainEvent domainEvent);
}
