namespace HabitTracker.dewsev;

class Program
{
    static void Main(string[] args)
    {
        HabitOccurrenceRepository habitOccurrenceRepository = new();

        MainMenu();
    }

    private static void MainMenu()
    {
        Console.WriteLine("Welcome to Habit Tracker!");
        Console.WriteLine("\n1.List all occurrences");
        Console.WriteLine("2.Add new occurrence");
        Console.WriteLine("3.Update occurrence");
        Console.WriteLine("4.Delete occurrence");
    }
}