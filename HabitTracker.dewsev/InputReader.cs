using System.Globalization;

namespace HabitTracker.dewsev;

public static class InputReader
{
    public static string GetString(string message, string? defaultValue = null)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine()?.Trim();
            
            if (defaultValue != null && string.IsNullOrEmpty(input))
            {
                return defaultValue;
            }
            
            if (!string.IsNullOrEmpty(input))
            {
                return input;
            }
            
            ConsoleHelpers.ClearCurrentConsoleLine();
        }
    }
    
    public static int GetNumeric(string message, int? defaultValue = null)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine()?.Trim();

            if (defaultValue != null && string.IsNullOrEmpty(input))
            {
                return defaultValue.Value;
            }
            
            if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int numericInput) && numericInput >= 0)
            {
                return numericInput;
            }
            
            ConsoleHelpers.ClearCurrentConsoleLine();
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
}