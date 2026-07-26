using System.Globalization;

namespace HabitTracker.dewsev;

class Program
{
    private static readonly HabitOccurrenceRepository _habitOccurrenceRepository = new();
    
    static void Main(string[] args)
    {

        MainMenu();
    }

    private static void MainMenu()
    {
        Console.Clear();
        Console.WriteLine("Welcome to Habit Tracker!");
        Console.WriteLine("\n1.List all occurrences");
        Console.WriteLine("2.Add new occurrence");
        Console.WriteLine("3.Update occurrence");
        Console.WriteLine("4.Delete occurrence");

        string? input = Console.ReadLine();

        switch (input)
        {
            case "2":
                AddOccurrence();
                break;
            default:
                Console.WriteLine("Invalid input.");
                break;
        }
    }

    private static void AddOccurrence()
    {
        Console.Clear();
        string date = GetDateInput("Provide a date (dd-MM-yyyy): ");
        int quantity = GetNumericInput("Provide quantity: ");

        _habitOccurrenceRepository.Insert(date, quantity);
    }

    private static int GetNumericInput(string message)
    {
        Console.Write(message);
        string? input = Console.ReadLine();
        int numericInput;
        while (!int.TryParse(input, out numericInput))
        {
            WriteLineColored("That's not a valid number. Try again.", ConsoleColor.Red);
            Console.Write(message);
            input = Console.ReadLine();
        }

        return numericInput;
    }
    
    private static string GetDateInput(string message)
    {
        Console.Write(message);
        string? input = Console.ReadLine();
        while (!DateTime.TryParseExact(input, "dd-MM-yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
        {
            WriteLineColored("Invalid date format. Try again.", ConsoleColor.Red);
            Console.Write(message);
            input = Console.ReadLine();
        }

        return input;
    }

    private static void WriteLineColored(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}