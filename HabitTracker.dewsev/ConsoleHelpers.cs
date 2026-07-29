namespace HabitTracker.dewsev;

public static class ConsoleHelpers
{
    public static void WriteColored(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(message);
        Console.ResetColor();
    }
}