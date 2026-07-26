
namespace HabitTracker.dewsev;

public record HabitOccurrence
{
    public int Id { get; init; }
    public DateTime Date { get; init; }
    public int Quantity { get; init; }
};