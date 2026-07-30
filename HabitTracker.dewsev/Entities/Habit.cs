namespace HabitTracker.dewsev;

public record Habit : IEntity
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string UnitOfMeasurement  { get; init; }
}