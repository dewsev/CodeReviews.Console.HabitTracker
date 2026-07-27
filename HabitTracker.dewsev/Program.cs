using System.Globalization;

namespace HabitTracker.dewsev;

class Program
{
    private const string DbConnectionString = "Data Source=HabitTracker.db";
    private const string DateFormat = "dd-MM-yyyy";
    private static readonly CultureInfo Culture = new("en-US");
    private static readonly HabitsRepository HabitsRepository = new(DbConnectionString);
    private static readonly HabitOccurrencesRepository HabitOccurrencesRepository = new(DbConnectionString, DateFormat, Culture);
    
    static void Main(string[] args)
    {
        while (true)
        {
            MainMenu();    
        }
    }

    private static void MainMenu()
    {
        Console.Clear();
        Console.WriteLine("Welcome to Habit Tracker!");
        Console.WriteLine("\n1.Create new habit");
        Console.WriteLine("2.List all occurrences");
        Console.WriteLine("3.Add new occurrence");
        Console.WriteLine("4.Update occurrence");
        Console.WriteLine("5.Delete occurrence");
        Console.WriteLine("6.Exit Application\n");
        
        Console.Write("Your choice: ");
        string? choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                // CreateNewHabit();
                break;
            case "2":
                ListAllOccurrences();
                Console.WriteLine("\nPress any key to go back to the Main Menu.");
                Console.ReadKey();
                break;
            case "3":
                AddOccurrence();
                break;
            case "4":
                UpdateOccurrence();
                break;
            case "5":
                DeleteOccurrence();
                break;
            case "6":
                Environment.Exit(0);
                break;
        }
    }
    
    private static void ListAllOccurrences()
    {
        Console.Clear();
        List<HabitOccurrence> occurrences = HabitOccurrencesRepository.GetAll();

        if (occurrences.Count == 0)
        {
            Console.WriteLine("You have not saved any occurrences yet.");
        }
        else
        {
            foreach (HabitOccurrence occurrence in occurrences)
            {
                WriteColored($"ID: {occurrence.Id}", ConsoleColor.Cyan);
                Console.Write(" - ");
                WriteColored(FormatDateTimeString(occurrence.Date), ConsoleColor.Cyan);
                Console.Write(" - ");
                WriteColored($"Quantity: {occurrence.Quantity}\n", ConsoleColor.Cyan);
            }
        }
    }
    
    private static void AddOccurrence()
    {
        Console.Clear();
        string date = GetDateInput($"Provide a date ({DateFormat} or \"now\"): ");
        int quantity = GetNumericInput("Provide quantity: ");

        HabitOccurrencesRepository.Insert(date, quantity);
    }

    private static void UpdateOccurrence()
    {
        ListAllOccurrences();
        Console.WriteLine();
        
        int id = GetNumericInput("Provide ID of an occurence that you want to update: ");
        
        HabitOccurrence? occurrence = HabitOccurrencesRepository.GetSingle(id);
        
        Console.Clear();
        if (occurrence == null)
        {
            WriteColored($"Occurrence with ID {id} was not found.", ConsoleColor.Red);
        }
        else
        {
            Console.WriteLine($"Editing Occurrence with ID {occurrence.Id}\n");
        
            string date = GetDateInput($"Provide new date (current: {FormatDateTimeString(occurrence.Date)}): ");
            int quantity = GetNumericInput($"Provide new quantity (current: {occurrence.Quantity}): ");
        
            HabitOccurrencesRepository.Update(id, date, quantity);
        
            Console.Clear();
            WriteColored($"Occurrence with ID {id} was successfully updated.\n", ConsoleColor.Green);
        }
        
        Console.WriteLine("\nPress any key to go back to the Main Menu.");
        Console.ReadKey();
    }
    
    private static void DeleteOccurrence()
    {
        ListAllOccurrences();
        Console.WriteLine();
        
        int occurrencesCount = HabitOccurrencesRepository.GetAll().Count;
        
        if (occurrencesCount != 0)
        {
            int id = GetNumericInput("Provide ID of an occurence that you want to delete: ");

            int deletedCount = HabitOccurrencesRepository.Delete(id);

            Console.Clear();
            if (deletedCount == 0)
            {
                WriteColored($"Occurrence with ID {id} was not found.", ConsoleColor.Red);
            }
            else
            {
                WriteColored($"Occurrence with ID {id} was successfully deleted.\n", ConsoleColor.Green);
            }
        }
        
        Console.WriteLine("\nPress any key to go back to Main Menu.");
        Console.ReadKey();
    }
    
    private static int GetNumericInput(string message)
    {
        Console.Write(message);
        string? input = Console.ReadLine();
        int numericInput;
        while (string.IsNullOrEmpty(input) || !int.TryParse(input, out numericInput) || Convert.ToInt32(input) < 0)
        {
            WriteColored("That's not a valid number. Try again.", ConsoleColor.Red);
            Console.Write($"\n{message}");
            input = Console.ReadLine();
        }

        return numericInput;
    }
    
    private static string GetDateInput(string message)
    {
        Console.Write(message);
        string? input = Console.ReadLine();

        if (input?.ToLower().Trim() == "now")
        {
            return DateTime.Now.ToString(DateFormat, Culture);
        }
        
        while (!DateTime.TryParseExact(input, DateFormat, Culture, DateTimeStyles.None, out _))
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
    
    private static string FormatDateTimeString(DateTime dateTime)
    {
        return dateTime.ToString(DateFormat, Culture);
    }
}