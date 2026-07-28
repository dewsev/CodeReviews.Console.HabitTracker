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
        Console.WriteLine("3.Edit habit");
        Console.WriteLine("4.List all habits with their occurrences");
        Console.WriteLine("5.Add new occurrence");
        Console.WriteLine("6.Update occurrence");
        Console.WriteLine("7.Delete occurrence");
        Console.WriteLine("8.Exit Application\n");
        
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
                EditHabitMenu();
                break;
            case "4":
                ListAllHabitsWithOccurrences();
                Console.WriteLine();
                AwaitKeyPress();
                break;
            case "5":
                AddOccurrence();
                break;
            case "6":
                UpdateOccurrence();
                break;
            case "7":
                DeleteOccurrence();
                break;
            case "8":
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
        WriteColored("Habit created!\n\n", ConsoleColor.Green);
        AwaitKeyPress();
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

        int id = GetIdChoiceInput("Select a habit to delete:\n", habits);
        
        HabitsRepository.Delete(id);
        
        Console.Clear();
        WriteColored("Habit successfully deleted.", ConsoleColor.Green);
        AwaitKeyPress();
    }

    private static void EditHabitMenu()
    {
        Console.Clear();
        
        List<Habit> habits = HabitsRepository.GetAll();
        
        if (habits.Count == 0)
        {
            Console.WriteLine("You have not created any habits yet.\n");
            AwaitKeyPress();
            return;
        }
        
        int id = GetIdChoiceInput("Select a habit to edit:\n", habits);
        
        Habit? habit = HabitsRepository.GetSingle(id);
        if (habit == null)
        {
            WriteColored("Habit not found.", ConsoleColor.Red);
            AwaitKeyPress();
            return;
        }
        
        Console.Clear();
        
        WriteColored($"Editing \"{habit.Name}\"...\n\n", ConsoleColor.Yellow);
        Console.WriteLine("Press ENTER to keep the old value\n");
        
        string name = GetStringInputWithDefault($"New habit name (current: {habit.Name}): ", habit.Name);
        string unitOfMeasurement = GetStringInputWithDefault($"New habit name (current: {habit.UnitOfMeasurement}): ", habit.UnitOfMeasurement);

        Console.Clear();
        
        bool success = HabitsRepository.Update(id, name, unitOfMeasurement);
        if (success)
        {
            WriteColored("Habit edited successfully.", ConsoleColor.Green);
        }
        else
        {
            WriteColored("Habit edit failed. Please try again.", ConsoleColor.Red);
        }
    }
    
    private static int CreateNewHabit()
    {
        string name = GetStringInput("New habit name: ");
        Console.WriteLine();
        string unitOfMeasurement = GetStringInput("Unit of measurement: ");
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
            while (!int.TryParse(readResult, out habitId) || habits.All(h => h.Id != habitId))
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
        
        AwaitKeyPress();
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

        AwaitKeyPress();
    }

    private static string GetStringInput(string message)
    {
        string? name = null;
        while (string.IsNullOrEmpty(name))
        {
            ClearCurrentConsoleLine();
            Console.Write(message);
            name = Console.ReadLine()?.Trim();
        }

        return name;
    }

    private static string GetStringInputWithDefault(string message, string defaultValue)
    {
        Console.Write(message);
        string? input = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(input))
        {
            input = defaultValue;
        }

        return input;
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

    private static int GetIdChoiceInput(string message, List<Habit> habits)
    {
        int id = -1;
        while (habits.All(h => h.Id != id))
        {
            Console.Clear();
            Console.WriteLine(message);
            ListHabits(habits);
            Console.WriteLine();
            
            id = GetNumericInput("Your choice: ");
        }

        return id;
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
        int cursorPosition = Console.CursorTop > 0 ? Console.CursorTop - 1 : Console.CursorTop;
        Console.SetCursorPosition(0, cursorPosition);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, cursorPosition);
    }

    private static void AwaitKeyPress()
    {
        Console.WriteLine("Press any key to go back to the Main Menu.");
        Console.ReadKey();
    }
}