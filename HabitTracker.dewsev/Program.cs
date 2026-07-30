using HabitTracker.dewsev.Entities;

namespace HabitTracker.dewsev;

class Program
{
    private const string DbConnectionString = "Data Source=HabitTracker.db";
    private static readonly HabitsRepository HabitsRepository = new(DbConnectionString);
    private static readonly OccurrencesRepository OccurrencesRepository = new(DbConnectionString);
    
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
        Console.WriteLine("\n1.Add a habit");
        Console.WriteLine("2.Delete a habit");
        Console.WriteLine("3.Edit a habit");
        Console.WriteLine("4.List all habits and their occurrences");
        Console.WriteLine("5.Add an occurrence");
        Console.WriteLine("6.Edit an occurrence");
        Console.WriteLine("7.Delete an occurrence");
        Console.WriteLine("8.Exit application\n");
        
        Console.Write("Your choice: ");
        string? choice = Console.ReadLine()?.Trim();
        switch (choice)
        {
            case "1":
                AddHabitMenu();
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
                AddOccurrenceMenu();
                break;
            case "6":
                EditOccurrenceMenu();
                break;
            case "7":
                DeleteOccurrenceMenu();
                break;
            case "8":
                Environment.Exit(0);
                break;
        }
    }

    private static void AddHabitMenu()
    {
        Console.Clear();
        Console.WriteLine("Your current habits:\n");
        
        List<Habit> habits = HabitsRepository.GetAll();
        if (habits.Count == 0)
        {
            ConsoleUi.ShowEmptyHabitsMessage(false);
        }
        else
        {
            ConsoleUi.ShowHabits(habits);
            Console.WriteLine();
        }

        Console.WriteLine("---------------------------------------------");
        Habit? habit = CreateHabit();
        if (habit is null) return;
        
        ConsoleUi.ShowSuccess("Habit created!");
        InputReader.AwaitAnyKeyPress();
    }

    private static void DeleteHabitMenu()
    {
        Console.Clear();
        
        List<Habit> habits = HabitsRepository.GetAll();
        if (habits.Count == 0)
        {
            ConsoleUi.ShowEmptyHabitsMessage();
            return;
        }

        Console.Clear();
        
        Habit? habit = InputReader.GetHabitChoice(habits);
        if (habit == null) return;
        
        HabitsRepository.Delete(habit.Id);
        ConsoleUi.ShowSuccess("Habit deleted!");
        InputReader.AwaitAnyKeyPress();
    }

    private static void EditHabitMenu()
    {
        Console.Clear();
        
        List<Habit> habits = HabitsRepository.GetAll();
        if (habits.Count == 0)
        {
            ConsoleUi.ShowEmptyHabitsMessage();
            return;
        }

        Habit? habit = InputReader.GetHabitChoice(habits);
        if (habit == null) return;
        
        Console.Clear();
        ConsoleUi.ShowInfo($"Editing habit \"{habit.Name}\"...");
        EditHabit(habit);
        ConsoleUi.ShowSuccess("Habit edited!");
        InputReader.AwaitAnyKeyPress();
    }
    
    
    private static void AddOccurrenceMenu()
    {
        Console.Clear();

        Habit? habit;
        List<Habit> habits = HabitsRepository.GetAll();
        if (habits.Count == 0)
        {
            ConsoleUi.ShowEmptyHabitsMessage(false);
            habit = CreateHabit();
        }
        else
        {
            habit = InputReader.GetHabitChoice(habits);
        }

        if (habit == null) return;
        
        Console.Clear();
        ConsoleUi.ShowInfo($"Adding an occurrence for \"{habit.Name}\"...");
        
        Occurrence? occurence = CreateNewOccurrence(habit.Id);
        if (occurence is null) return;
        
        ConsoleUi.ShowSuccess("Occurrence added!");
        InputReader.AwaitAnyKeyPress();
    }

    private static void EditOccurrenceMenu()
    {
        List<Habit> habits = HabitsRepository.GetAll();
        if (habits.Count == 0)
        {
            ConsoleUi.ShowEmptyHabitsMessage();
            return;
        }

        Console.Clear();
        
        Habit? habit = InputReader.GetHabitChoice(habits);
        if (habit == null) return;
        
        Console.Clear();
        
        List<Occurrence> occurrences = OccurrencesRepository.GetAllByHabitId(habit.Id);
        if (occurrences.Count == 0)
        {
            ConsoleUi.ShowEmptyOccurrencesMessage();
            return;
        }
        
        ConsoleUi.ShowInfo($"Editing an occurrence for \"{habit.Name}\"...");
        Occurrence? occurrence = InputReader.GetOccurrenceChoice(occurrences, habit.UnitOfMeasurement);
        if (occurrence == null) return;
        
        Console.Clear();
        ConsoleUi.ShowInfo($"Editing occurrence with ID {occurrence.Id} for \"{habit.Name}\"...");
        EditOccurrence(occurrence);
        ConsoleUi.ShowSuccess("Occurrence edited!");
        InputReader.AwaitAnyKeyPress();
    }
    
    private static void DeleteOccurrenceMenu()
    {
        Console.Clear();
        
        List<Habit> habits = HabitsRepository.GetAll();
        if (habits.Count == 0)
        {
            ConsoleUi.ShowEmptyHabitsMessage();
            return;
        }
    
        Console.Clear();

        Habit? habit = InputReader.GetHabitChoice(habits);
        if (habit == null) return;
        
        Console.Clear();
        
        List<Occurrence> occurrences = OccurrencesRepository.GetAllByHabitId(habit.Id);
        if (occurrences.Count == 0)
        {
            ConsoleUi.ShowEmptyOccurrencesMessage();
            return;
        }

        ConsoleUi.ShowInfo($"Deleting an occurrence for \"{habit.Name}\"...");
        Occurrence? occurrence = InputReader.GetOccurrenceChoice(occurrences, habit.UnitOfMeasurement);
        if (occurrence == null) return;
    
        OccurrencesRepository.Delete(occurrence.Id);
        ConsoleUi.ShowSuccess("Occurrence deleted!");
        InputReader.AwaitAnyKeyPress();
    }
    
    private static void ListAllHabitsWithOccurrences()
    {
        Console.Clear();
        List<Habit> habits = HabitsRepository.GetAll();
        List<Occurrence> occurrences = OccurrencesRepository.GetAll();

        if (habits.Count == 0)
        {
            ConsoleUi.ShowEmptyHabitsMessage(false);
        }
        else
        {
            ConsoleUi.ShowHabitsWithOccurrences(habits, occurrences);
        }
    }
    
    private static void EditOccurrence(Occurrence occurrence)
    {
        string date = InputReader.GetDateWithFallback(
            $"Provide new date (ENTER = {DateFormatter.FormatDateTime(occurrence.Date)}): ", 
            occurrence.Date
        );
        
        int quantity = InputReader.GetNumericWithFallback(
            $"Provide new quantity (ENTER = {occurrence.Quantity}): ",
            occurrence.Quantity
        );
        
        OccurrencesRepository.Update(occurrence.Id, date, quantity);
    }

    private static Occurrence? CreateNewOccurrence(int habitId)
    {
        string date = InputReader.GetDateWithFallback(
            $"Provide a date ({DateFormatter.DateFormat} or ENTER for today): ", 
            DateTime.Now
        );
        
        int? quantity = InputReader.GetNumericNullable("Provide quantity (ENTER = Main Menu): ");
        if (quantity is null) return null;

        return OccurrencesRepository.Insert(date, quantity.Value, habitId);
    }
    
    private static Habit? CreateHabit()
    {
        ConsoleUi.ShowInfo("Creating new habit...");
        string? name = InputReader.GetStringNullable("Name (ENTER = Main Menu): ");
        if (name is null) return null;
        
        string? unitOfMeasurement = InputReader.GetStringNullable("Unit of measurement (ENTER = Main Menu): ");
        if (unitOfMeasurement is null) return null;
        
        return HabitsRepository.Insert(name, unitOfMeasurement);
    }

    private static void EditHabit(Habit habit)
    {
        string name = InputReader.GetStringWithFallback($"Name (ENTER = {habit.Name}): ", habit.Name);
        string unitOfMeasurement = InputReader.GetStringWithFallback($"Unit of measurement (ENTER = {habit.UnitOfMeasurement}): ", habit.UnitOfMeasurement);

        HabitsRepository.Update(habit.Id, name, unitOfMeasurement);
    }
}