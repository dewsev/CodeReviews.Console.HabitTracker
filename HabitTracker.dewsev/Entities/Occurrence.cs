
namespace HabitTracker.dewsev.Entities;

public record Occurrence : IEntity
{
    public required int Id { get; init; }
    public required int HabitId { get; init; }
    public required DateTime Date { get; init; }
    public required int Quantity { get; init; }
}