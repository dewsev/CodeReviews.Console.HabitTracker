namespace HabitTracker.dewsev;

public record OccurrenceWithHabit : IEntity
{
    public required int Id { get; init; }
    public required int HabitId { get; init; }
    public required string HabitName { get; init; }
    public required string UnitOfMeasurement { get; init; }
    public required DateTime Date { get; init; }
    public required int Quantity { get; init; }
};