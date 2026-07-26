using System.Globalization;

namespace HabitTracker.dewsev;

class Program
{
    private static readonly HabitOccurrenceRepository HabitOccurrenceRepository = new();
    
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
            case "1":
                ListAllOccurrences();
                break;
            case "2":
                AddOccurrence();
                break;
            default:
                Console.WriteLine("Invalid input.");
                break;
        }
    }

    private static void ListAllOccurrences()
    {
        Console.Clear();
        List<HabitOccurrence> occurrences = HabitOccurrenceRepository.GetAll();

        foreach (HabitOccurrence occurrence in occurrences)
        {
            WriteColored($"\nID: {occurrence.Id}", ConsoleColor.Cyan);
            Console.Write(" - ");
            WriteColored(occurrence.Date.ToString("dd-MM-yyyy", new CultureInfo("en-US")), ConsoleColor.Cyan);
            Console.Write(" - ");
            WriteColored($"Quantity: {occurrence.Quantity}", ConsoleColor.Cyan);
        }
    }
    
    private static void AddOccurrence()
    {
        Console.Clear();
        string date = GetDateInput("Provide a date (dd-MM-yyyy): ");
        int quantity = GetNumericInput("Provide quantity: ");

        HabitOccurrenceRepository.Insert(date, quantity);
    }

    private static int GetNumericInput(string message)
    {
        Console.Write(message);
        string? input = Console.ReadLine();
        int numericInput;
        while (!int.TryParse(input, out numericInput))
        {
            WriteColored("That's not a valid number. Try again.\n", ConsoleColor.Red);
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
            WriteColored("Invalid date format. Try again.\n", ConsoleColor.Red);
            Console.Write(message);
            input = Console.ReadLine();
        }

        return input;
    }

    private static void WriteColored(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(message);
        Console.ResetColor();
    }
}