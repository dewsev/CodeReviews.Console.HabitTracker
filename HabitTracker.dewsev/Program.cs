using HabitTracker.dewsev.Entities;
using Microsoft.Data.Sqlite;

namespace HabitTracker.dewsev;

class Program
{
    private const string DbConnectionString = "Data Source=HabitTracker.db";
    private static readonly DbInitializer DbInitializer = new(DbConnectionString);
    private static readonly HabitsRepository HabitsRepository = new(DbConnectionString);
    private static readonly OccurrencesRepository OccurrencesRepository = new(DbConnectionString);
    
    static void Main(string[] args)
    {
        try
        {
            DbInitializer.Initialize();
        }
        catch (SqliteException ex)
        {
            ConsoleHelpers.WriteErrorMessage("Failed to create DB. Application exiting.");
            Environment.Exit(ex.ErrorCode);
        }
        
        
        while (true)
        {
            MainMenu();    
        }
    }

    private static void MainMenu()
    {
        Console.Clear();
        Console.WriteLine("Welcome to Habit Tracker!");
        Console.WriteLine("\n1.Show all habits");
        Console.WriteLine("2.Show one habit");
        Console.WriteLine("3.Add a habit");
        Console.WriteLine("4.Edit a habit");
        Console.WriteLine("5.Delete a habit");
        Console.WriteLine("6.Add an occurrence");
        Console.WriteLine("7.Edit an occurrence");
        Console.WriteLine("8.Delete an occurrence");
        Console.WriteLine("9.Exit application\n");
        
        Console.Write("Your choice: ");
        string? choice = Console.ReadLine()?.Trim();
        switch (choice)
        {
            case "1":
                AllHabitsMenu();
                break;
            case "2":
                SingleHabitMenu();
                break;
            case "3":
                AddHabitMenu();
                break;
            case "4":
                EditHabitMenu();
                break;
            case "5":
                DeleteHabitMenu();
                break;
            case "6":
                AddOccurrenceMenu();
                break;
            case "7":
                EditOccurrenceMenu();
                break;
            case "8":
                DeleteOccurrenceMenu();
                break;
            case "9":
                Environment.Exit(0);
                break;
        }
    }
    private static void AllHabitsMenu()
    {
        Console.Clear();
        List<Habit> habits = HabitsRepository.GetAll();
        List<Occurrence> occurrences = OccurrencesRepository.GetAll();

        if (habits.Count == 0)
        {
            ConsoleRenderer.RenderEmptyHabitListMessage();
        }
        else
        {
            ConsoleRenderer.RenderHabitListWithOccurrences(habits, occurrences);
        }
        
        InputReader.AwaitAnyKeyPress();
    }

    private static void SingleHabitMenu()
    {
        Console.Clear();
        
        List<Habit> habits = HabitsRepository.GetAll();
        if (habits.Count == 0)
        {
            ConsoleRenderer.RenderEmptyHabitListMessage();
            InputReader.AwaitAnyKeyPress();
            return;
        }

        Habit? habit = InputReader.GetHabitChoice(habits);
        if (habit is null) return;
            
        List<Occurrence> occurrences = OccurrencesRepository.GetAllByHabitId(habit.Id);
        
        Console.Clear();
        ConsoleRenderer.RenderHabitWithOccurrences(habit, occurrences);
        
        Console.WriteLine();
        InputReader.AwaitAnyKeyPress();
    }
    
    private static void AddHabitMenu()
    {
        Console.Clear();
        Console.WriteLine("Your current habits:\n");
        
        List<Habit> habits = HabitsRepository.GetAll();
        if (habits.Count == 0)
        {
            ConsoleRenderer.RenderEmptyHabitListMessage();
        }
        else
        {
            ConsoleRenderer.RenderHabitList(habits);
            Console.WriteLine();
        }

        Console.WriteLine("---------------------------------------------");
        ConsoleHelpers.WriteInfoMessage("Creating new habit...");
        
        Habit? habit = CreateHabit();
        if (habit is null) return;
        
        ConsoleHelpers.WriteSuccessMessage("Habit created!");
        InputReader.AwaitAnyKeyPress();
    }

    private static void DeleteHabitMenu()
    {
        Console.Clear();
        
        List<Habit> habits = HabitsRepository.GetAll();
        if (habits.Count == 0)
        {
            ConsoleRenderer.RenderEmptyHabitListMessage();
            InputReader.AwaitAnyKeyPress();
            return;
        }
        
        Console.Clear();
        
        Habit? habit = InputReader.GetHabitChoice(habits);
        if (habit is null) return;
    
        HabitsRepository.Delete(habit.Id);

        ConsoleHelpers.WriteSuccessMessage("Habit deleted!");    
        InputReader.AwaitAnyKeyPress();
    }

    private static void EditHabitMenu()
    {
        Console.Clear();
        
        List<Habit> habits = HabitsRepository.GetAll();
        if (habits.Count == 0)
        {
            ConsoleRenderer.RenderEmptyHabitListMessage();
            InputReader.AwaitAnyKeyPress();
            return;
        }
        
        Habit? habit = InputReader.GetHabitChoice(habits);
        if (habit is null) return;
    
        Console.Clear();
        ConsoleHelpers.WriteInfoMessage($"Editing habit \"{habit.Name}\"...");
        
        EditHabit(habit);
        
        ConsoleHelpers.WriteSuccessMessage("Habit edited!");    
        InputReader.AwaitAnyKeyPress();
    }
    
    private static void AddOccurrenceMenu()
    {
        Console.Clear();

        List<Habit> habits = HabitsRepository.GetAll();
        if (habits.Count == 0)
        {
            ConsoleRenderer.RenderEmptyHabitListMessage();
            InputReader.AwaitAnyKeyPress();
            return;
        }
        
        Habit? habit = InputReader.GetHabitChoice(habits);
        if (habit is null) return;
    
        Console.Clear();
        ConsoleHelpers.WriteInfoMessage($"Adding an occurrence for \"{habit.Name}\"...");
    
        Occurrence? occurence = CreateOccurrence(habit.Id);
        if (occurence is null) return;
    
        ConsoleHelpers.WriteSuccessMessage("Occurrence added!");    
        InputReader.AwaitAnyKeyPress();
    }

    private static void EditOccurrenceMenu()
    {
        List<Habit> habits = HabitsRepository.GetAll();
        if (habits.Count == 0)
        {
            ConsoleRenderer.RenderEmptyHabitListMessage();
            InputReader.AwaitAnyKeyPress();
            return;
        }
        
        Console.Clear();
    
        Habit? habit = InputReader.GetHabitChoice(habits);
        if (habit is null) return;
    
        Console.Clear();
    
        List<Occurrence> occurrences = OccurrencesRepository.GetAllByHabitId(habit.Id);
        if (occurrences.Count == 0)
        {
            ConsoleRenderer.RenderEmptyOccurrenceListMessage();
            InputReader.AwaitAnyKeyPress();
            return;
        }
        
        ConsoleHelpers.WriteInfoMessage($"Editing an occurrence for \"{habit.Name}\"...");
        Occurrence? occurrence = InputReader.GetOccurrenceChoice(occurrences, habit.UnitOfMeasurement);
        if (occurrence is null) return;

        Console.Clear();
        ConsoleHelpers.WriteInfoMessage($"Editing occurrence with ID {occurrence.Id} for \"{habit.Name}\"...");
        
        EditOccurrence(occurrence);
        
        ConsoleHelpers.WriteSuccessMessage("Occurrence edited!");    
        InputReader.AwaitAnyKeyPress();
    }
    
    private static void DeleteOccurrenceMenu()
    {
        Console.Clear();
        
        List<Habit> habits = HabitsRepository.GetAll();
        if (habits.Count == 0)
        {
            ConsoleRenderer.RenderEmptyHabitListMessage();
            InputReader.AwaitAnyKeyPress();
            return;
        }
    
        Console.Clear();

        Habit? habit = InputReader.GetHabitChoice(habits);
        if (habit is null) return;
        
        Console.Clear();
        
        List<Occurrence> occurrences = OccurrencesRepository.GetAllByHabitId(habit.Id);
        if (occurrences.Count == 0)
        {
            ConsoleRenderer.RenderEmptyOccurrenceListMessage();
            InputReader.AwaitAnyKeyPress();
            return;
        }

        ConsoleHelpers.WriteInfoMessage($"Deleting an occurrence for \"{habit.Name}\"...");
        Occurrence? occurrence = InputReader.GetOccurrenceChoice(occurrences, habit.UnitOfMeasurement);
        if (occurrence is null) return;
    
        OccurrencesRepository.Delete(occurrence.Id);
        ConsoleHelpers.WriteSuccessMessage("Occurrence deleted!");
        InputReader.AwaitAnyKeyPress();
    }
    
    private static Habit? CreateHabit()
    {
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
    
    private static Occurrence? CreateOccurrence(int habitId)
    {
        string date = InputReader.GetDateWithFallback(
            $"Provide a date ({DateParser.DateFormat} or ENTER for today): ", 
            DateTime.Now
        );
        
        int? quantity = InputReader.GetNumericNullable("Provide quantity (ENTER = Main Menu): ");
        if (quantity is null) return null;

        return OccurrencesRepository.Insert(date, quantity.Value, habitId);
    }
    
    private static void EditOccurrence(Occurrence occurrence)
        {
            string date = InputReader.GetDateWithFallback(
                $"Provide new date (ENTER = {DateParser.GetDateTimeString(occurrence.Date)}): ", 
                occurrence.Date
            );
            
            int quantity = InputReader.GetNumericWithFallback(
                $"Provide new quantity (ENTER = {occurrence.Quantity}): ",
                occurrence.Quantity
            );
            
            OccurrencesRepository.Update(occurrence.Id, date, quantity);
        }
    
}