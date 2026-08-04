namespace HabitTracker.dewsev.Entities;

public record Habit : IEntity
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string UnitOfMeasurement  { get; init; }
}