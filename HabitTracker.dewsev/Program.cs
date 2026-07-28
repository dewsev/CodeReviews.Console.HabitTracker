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
        Console.WriteLine("\n1.Add new habit");
        Console.WriteLine("2.Remove habit");
        Console.WriteLine("3.List all habits with their occurrences");
        Console.WriteLine("4.Add new occurrence");
        Console.WriteLine("5.Update occurrence");
        Console.WriteLine("6.Delete occurrence");
        Console.WriteLine("7.Exit Application\n");
        
        Console.Write("Your choice: ");
        string? choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                AddNewHabitMenu();
                break;
            case "2":
                DeleteHabitMenu();
                break;
            case "3":
                ListAllHabitsWithOccurrences();
                Console.WriteLine("\nPress any key to go back to the Main Menu.");
                Console.ReadKey();
                break;
            case "4":
                AddOccurrence();
                break;
            case "5":
                UpdateOccurrence();
                break;
            case "6":
                DeleteOccurrence();
                break;
            case "7":
                Environment.Exit(0);
                break;
        }
    }

    private static void AddNewHabitMenu()
    {
        Console.Clear();
        
        List<Habit> habits = HabitsRepository.GetAll();
        
        if (habits.Count > 0)
        {
            Console.WriteLine("Your current habits:\n");
            ListHabits(habits);
            Console.WriteLine("\n");
        }
        else
        {
            Console.WriteLine("You have not created any habits yet.\n\n");
        }

        CreateNewHabit();


        Console.Clear();
        WriteColored("Habit created!", ConsoleColor.Green);
        Console.WriteLine("\nPress any key to go back to the Main Menu.");
        Console.ReadKey();
    }

    private static void DeleteHabitMenu()
    {
        Console.Clear();
        
        List<Habit> habits = HabitsRepository.GetAll();
        
        if (habits.Count == 0)
        {
            Console.WriteLine("You have not created any habits yet.\n\n");
            return;
        }
        
        int idChoice = -1;
        while (habits.FindIndex(h => h.Id == idChoice) == -1)
        {
            Console.Clear();
            Console.WriteLine("Select a habit to delete:\n");
            ListHabits(habits);
            Console.WriteLine();
            
            idChoice = GetNumericInput("Your choice: ");
        }
        
        HabitsRepository.Delete(idChoice);
        
        Console.Clear();
        WriteColored("Habit successfully deleted.", ConsoleColor.Green);
        Console.WriteLine("\n\nPress any key to go back to the Main Menu.");
        Console.ReadKey();
    }
    
    private static int CreateNewHabit()
    {
        string? name = null;
        while (string.IsNullOrEmpty(name))
        {
            ClearCurrentConsoleLine();
            Console.Write("New habit name: ");
            name = Console.ReadLine()?.Trim();
        }
        
        Console.WriteLine();
        
        string? unitOfMeasurement = null;
        while (string.IsNullOrEmpty(unitOfMeasurement))
        {
            ClearCurrentConsoleLine();
            Console.Write("Unit of measurement: ");
            unitOfMeasurement = Console.ReadLine()?.Trim();
        }
        
        return HabitsRepository.Insert(name, unitOfMeasurement);
    }
    
    private static void ListAllHabitsWithOccurrences()
    {
        Console.Clear();
        List<Habit> habits = HabitsRepository.GetAll();
        List<HabitOccurrence> occurrences = HabitOccurrencesRepository.GetAll();

        if (habits.Count == 0)
        {
            Console.WriteLine("You have not created any habits yet.");
        }
        else
        {
            foreach (Habit habit in habits)
            {
                WriteColored($"{habit.Id}.", ConsoleColor.Cyan);
                Console.Write($"{habit.Name}\n");

                List<HabitOccurrence> currentHabitOccurrences = occurrences.FindAll(o => o.HabitId == habit.Id);
                
                for (int i = 0; i < currentHabitOccurrences.Count; i++)
                {
                    HabitOccurrence occurrence = currentHabitOccurrences[i];
                    
                    string asciiCharacter = i == currentHabitOccurrences.Count - 1 ? "└──" : "├──";
                    Console.Write($"{asciiCharacter} {FormatDateTimeString(occurrence.Date)}");
                    Console.Write(" — ");
                    Console.Write($"{occurrence.Quantity} {habit.UnitOfMeasurement}\n");
                }
            }
        }
    }

    private static void ListHabits(List<Habit> habits)
    {
        foreach (Habit habit in habits)
        {
            WriteColored($"{habit.Id}.", ConsoleColor.Cyan);
            Console.Write($"{habit.Name}\n");
        }
    }
    
    private static void AddOccurrence()
    {
        Console.Clear();

        int habitId;
        
        List<Habit> habits = HabitsRepository.GetAll();
        
        if (habits.Count == 0)
        {
            Console.WriteLine("You have not created any habits yet.\n");
            habitId = CreateNewHabit();
        }
        else
        {
            string? readResult = null;
            while (!int.TryParse(readResult, out habitId) || habits.FindIndex(h => h.Id == habitId) == -1)
            {
                Console.Clear();
                Console.Write("Select a habit: \n\n");
                ListHabits(habits);
                Console.Write("\nYour choice: ");
                readResult = Console.ReadLine()?.Trim();
            }    
        }

        Console.Clear();
        string date = GetDateInput($"Provide a date ({DateFormat} or \"now\"): ");
        int quantity = GetNumericInput("Provide quantity: ");

        HabitOccurrencesRepository.Insert(date, quantity, habitId);
    }

    private static void UpdateOccurrence()
    {
        // TODO: Update this method
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
        // TODO: Update this method
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

        // TODO: Empty input should return today's date
        if (input?.ToLower().Trim() == "now")
        {
            return FormatDateTimeString(DateTime.Now);
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
    
    private static void ClearCurrentConsoleLine()
    {
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, Console.CursorTop - 1);
    }
}