namespace HabitTracker.dewsev;

public static class ConsoleHelpers
{
    public static void WriteColored(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(message);
        Console.ResetColor();
    }
    
    public static void WriteSuccessMessage(string message)
    {
        WriteColored($"\n{message}\n\n", ConsoleColor.Green);
    }

    public static void WriteInfoMessage(string message)
    {
        WriteColored($"{message}\n\n", ConsoleColor.Cyan);
    }
    
    public static void ClearCurrentConsoleLine()
    {
        int cursorPosition = Console.CursorTop > 0 ? Console.CursorTop - 1 : Console.CursorTop;
        Console.SetCursorPosition(0, cursorPosition);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, cursorPosition);
    }
}