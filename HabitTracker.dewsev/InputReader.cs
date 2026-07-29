using System.Globalization;

namespace HabitTracker.dewsev;

public static class InputReader
{
    public static string GetString(string message)
    {
        while (true)
        {
            Console.Write(message);
            string? name = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(name))
            {
                return name;
            }
            
            ClearCurrentConsoleLine();
        }
    }
    
    public static string GetStringWithDefault(string message, string defaultValue)
    {
        Console.Write(message);
        string? input = Console.ReadLine()?.Trim();
        return string.IsNullOrEmpty(input) ? defaultValue : input;
    }
    
    public static int GetNumeric(string message, int? defaultValue = null)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input) && defaultValue != null)
            {
                return defaultValue.Value;
            }
            
            if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int numericInput) && numericInput >= 0)
            {
                return numericInput;
            }
            
            ClearCurrentConsoleLine();
        }
    }
    
    public static string GetDate(string message, string format, CultureInfo culture, DateTime? defaultValue = null)
    {
        Console.Write(message);
        string? input = Console.ReadLine();

        if (string.IsNullOrEmpty(input))
        {
            return DateFormatter.FormatDateTimeString(defaultValue ?? DateTime.Now);
        }
        
        while (!DateTime.TryParseExact(input, format, culture, DateTimeStyles.None, out _))
        {
            ConsoleHelpers.WriteColored("Invalid date format. Try again.\n", ConsoleColor.Red);
            Console.Write(message);
            input = Console.ReadLine();
        }

        return input;
    }
    
    public static void AwaitAnyKeyPress()
    {
        Console.WriteLine("Press any key to go back to the Main Menu.");
        Console.ReadKey();
    }
    
    private static void ClearCurrentConsoleLine()
    {
        int cursorPosition = Console.CursorTop > 0 ? Console.CursorTop - 1 : Console.CursorTop;
        Console.SetCursorPosition(0, cursorPosition);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, cursorPosition);
    }
}