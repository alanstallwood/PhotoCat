namespace PhotoCat.Application.Interfaces;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
