using System.Globalization;
using HabitTracker.dewsev.Entities;

namespace HabitTracker.dewsev;

public static class InputReader
{
    public static string? GetStringNullable(string message)
    {
        Console.Write(message);
        string? input = Console.ReadLine()?.Trim();
        return string.IsNullOrEmpty(input) ? null : input;
    }
    
    public static string GetStringWithFallback(string message, string fallbackValue)
    {
        Console.Write(message);
        string? input = Console.ReadLine()?.Trim();
        return string.IsNullOrEmpty(input) ? fallbackValue : input;
    }
    
    public static int GetNumericWithFallback(string message, int fallbackValue)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
            {
                return fallbackValue;
            }
            
            if (int.TryParse(input, out int numericInput) && numericInput >= 0)
            {
                return numericInput;
            }
            
            ConsoleHelpers.ClearCurrentConsoleLine();
        }
    }
    
    public static string GetDateWithFallback(string message, DateTime fallbackValue)
    {
        Console.Write(message);
        string? input = Console.ReadLine();

        if (string.IsNullOrEmpty(input))
        {
            return DateParser.GetDateTimeString(fallbackValue);
        }
        
        while (!DateParser.IsValidDateFormat(input))
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
        return GetEntityChoice(habits, ConsoleRenderer.RenderHabitList, "Select a habit:");
    }

    public static Occurrence? GetOccurrenceChoice(List<Occurrence> occurrences, string unitOfMeasurement)
    {
        return GetEntityChoice(
            occurrences,
            (o) => ConsoleRenderer.RenderOccurrenceListWithIds(o, unitOfMeasurement)
        );
    }
    
    public static int? GetNumericNullable(string message)
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
            int? id = GetNumericNullable("Your choice (ENTER = Main Menu): ");
            if (id is null)
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