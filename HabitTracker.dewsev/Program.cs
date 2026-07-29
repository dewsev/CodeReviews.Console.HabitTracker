using System.Globalization;

namespace HabitTracker.dewsev;

class Program
{
    private const string DbConnectionString = "Data Source=HabitTracker.db";
    private static readonly HabitsRepository HabitsRepository = new(DbConnectionString);
    private static readonly HabitOccurrencesRepository HabitOccurrencesRepository = new(DbConnectionString);
    
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
                InputReader.AwaitAnyKeyPress();
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
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("You have not created any habits yet.\n\n");
        }

        CreateNewHabit();

        Console.Clear();
        ConsoleHelpers.WriteColored("Habit created!\n\n", ConsoleColor.Green);
        InputReader.AwaitAnyKeyPress();
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

        int id = GetHabitIdChoice("Select a habit to delete:", habits);
        
        bool success = HabitsRepository.Delete(id);

        Console.Clear();
        if (success)
        {
            ConsoleHelpers.WriteColored("Habit successfully deleted.\n\n", ConsoleColor.Green);
        }
        else
        {
            ConsoleHelpers.WriteColored("Failed to delete. Please try again.\n\n", ConsoleColor.Red);
        }
        
        InputReader.AwaitAnyKeyPress();
    }

    private static void EditHabitMenu()
    {
        Console.Clear();
        
        List<Habit> habits = HabitsRepository.GetAll();
        
        if (habits.Count == 0)
        {
            Console.WriteLine("You have not created any habits yet.\n");
            InputReader.AwaitAnyKeyPress();
            return;
        }
        
        int id = GetHabitIdChoice("Select a habit to edit:", habits);
        
        Habit? habit = HabitsRepository.GetSingle(id);
        if (habit == null)
        {
            ConsoleHelpers.WriteColored("Habit not found.", ConsoleColor.Red);
            InputReader.AwaitAnyKeyPress();
            return;
        }
        
        Console.Clear();
        
        ConsoleHelpers.WriteColored($"Editing \"{habit.Name}\"...\n\n", ConsoleColor.Yellow);
        Console.WriteLine("Press ENTER to keep the old value\n");
        
        string name = InputReader.GetStringWithDefault($"New habit name (current: {habit.Name}): ", habit.Name);
        string unitOfMeasurement = InputReader.GetStringWithDefault($"New habit name (current: {habit.UnitOfMeasurement}): ", habit.UnitOfMeasurement);

        Console.Clear();
        
        bool success = HabitsRepository.Update(id, name, unitOfMeasurement);
        if (success)
        {
            ConsoleHelpers.WriteColored("Habit edited successfully.", ConsoleColor.Green);
        }
        else
        {
            ConsoleHelpers.WriteColored("Habit edit failed. Please try again.", ConsoleColor.Red);
        }
    }
    
    private static int CreateNewHabit()
    {
        string name = InputReader.GetString("New habit name: ");
        string unitOfMeasurement = InputReader.GetString("Unit of measurement: ");
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
                ConsoleHelpers.WriteColored($"{habit.Id}.", ConsoleColor.Cyan);
                Console.Write($"{habit.Name}\n");

                List<HabitOccurrence> currentHabitOccurrences = occurrences.FindAll(o => o.HabitId == habit.Id);

                if (currentHabitOccurrences.Count == 0)
                {
                    Console.WriteLine("└── There are no occurrences logged for this habit yet.");
                    continue;
                }
                
                for (int i = 0; i < currentHabitOccurrences.Count; i++)
                {
                    HabitOccurrence occurrence = currentHabitOccurrences[i];
                    
                    string asciiCharacter = i == currentHabitOccurrences.Count - 1 ? "└──" : "├──";
                    Console.Write($"{asciiCharacter} {DateFormatter.FormatDateTimeString(occurrence.Date)}");
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
            ConsoleHelpers.WriteColored($"{habit.Id}.", ConsoleColor.Cyan);
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
            habitId = GetHabitIdChoice("Select a habit:", habits);
        }

        Console.Clear();
        string date = InputReader.GetDate($"Provide a date ({DateFormatter.DateFormat} or \"now\"): ", DateFormatter.DateFormat, DateFormatter.Culture);
        int quantity = InputReader.GetNumeric("Provide quantity: ");

        HabitOccurrencesRepository.Insert(date, quantity, habitId);
    }

    private static void UpdateOccurrence()
    {
        // TODO: Update this method
        Console.WriteLine();
        
        int id = InputReader.GetNumeric("Provide ID of an occurence that you want to update: ");
        
        HabitOccurrence? occurrence = HabitOccurrencesRepository.GetSingle(id);
        
        Console.Clear();
        if (occurrence == null)
        {
            ConsoleHelpers.WriteColored($"Occurrence with ID {id} was not found.", ConsoleColor.Red);
        }
        else
        {
            Console.WriteLine($"Editing Occurrence with ID {occurrence.Id}\n");
        
            string date = InputReader.GetDate($"Provide new date (current: {DateFormatter.FormatDateTimeString(occurrence.Date)}): ", DateFormatter.DateFormat, DateFormatter.Culture);
            int quantity = InputReader.GetNumeric($"Provide new quantity (current: {occurrence.Quantity}): ");
        
            HabitOccurrencesRepository.Update(id, date, quantity);
        
            Console.Clear();
            ConsoleHelpers.WriteColored($"Occurrence with ID {id} was successfully updated.\n", ConsoleColor.Green);
        }
        
        InputReader.AwaitAnyKeyPress();
    }
    
    private static void DeleteOccurrence()
    {
        // TODO: Update this method
        Console.WriteLine();
        
        int occurrencesCount = HabitOccurrencesRepository.GetAll().Count;
        
        if (occurrencesCount != 0)
        {
            int id = InputReader.GetNumeric("Provide ID of an occurence that you want to delete: ");

            int deletedCount = HabitOccurrencesRepository.Delete(id);

            Console.Clear();
            if (deletedCount == 0)
            {
                ConsoleHelpers.WriteColored($"Occurrence with ID {id} was not found.", ConsoleColor.Red);
            }
            else
            {
                ConsoleHelpers.WriteColored($"Occurrence with ID {id} was successfully deleted.\n", ConsoleColor.Green);
            }
        }

        InputReader.AwaitAnyKeyPress();
    }
    
    public static int GetHabitIdChoice(string message, List<Habit> habits)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"{message}\n");
            ListHabits(habits);
            Console.WriteLine();
            
            int id = InputReader.GetNumeric("Your choice: ");
            
            if (habits.Any(h => h.Id == id))
            {
                return id;
            }
        }
    }
}