using System.Globalization;
using HabitTracker.dewsev.Entities;

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
            return DateFormatter.FormatDateTime(defaultValue ?? DateTime.Now);
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
    
    public static Habit? GetHabitChoice(List<Habit> habits)
    {
        return GetEntityChoice(habits, ConsoleUi.ShowHabits, "Select a habit:");
    }

    public static Occurrence? GetOccurrenceChoice(List<Occurrence> occurrences, string unitOfMeasurement)
    {
        return GetEntityChoice(
            occurrences,
            (o) => ConsoleUi.ShowOccurrences(o, unitOfMeasurement)
        );
    }
    
    private static int? GetNumericNullable(string message)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
            {
                return null;
            }
            
            if (int.TryParse(input, out int numericInput) && numericInput >= 0)
            {
                return numericInput;
            }
            
            ConsoleHelpers.ClearCurrentConsoleLine();
        }
    }
    
    private static T? GetEntityChoice<T>(List<T> entities, Action<List<T>> displayDelegate, string? message = null) where T : class, IEntity
    {
        if (message != null)
        {
            Console.WriteLine($"{message}\n");    
        }
        
        displayDelegate(entities);
        Console.WriteLine();
        
        while (true)
        {
            int? id = GetNumericNullable("Your choice (press ENTER to go back to the Main Menu): ");
            if (id == null)
            {
                return null;
            }
            
            var chosenEntity = entities.Find(e => e.Id == id);
            if (chosenEntity != null)
            {
                return chosenEntity;
            }
            
            ConsoleHelpers.ClearCurrentConsoleLine();
        }
    }
}