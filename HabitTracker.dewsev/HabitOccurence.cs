
namespace HabitTracker.dewsev;

public record HabitOccurrence
{
    public required int Id { get; init; }
    public required int HabitId { get; init; }
    public required DateTime Date { get; init; }
    public required int Quantity { get; init; }
}