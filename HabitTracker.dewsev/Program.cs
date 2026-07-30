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
                AddOccurrenceMenu();
                break;
            case "6":
                EditOccurrenceMenu();
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
        
        Console.WriteLine("Your current habits:\n");
        if (habits.Count > 0)
        {
            ListHabits(habits);
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("You have not created any habits yet.\n");
        }
        
        CreateNewHabit();

        ConsoleHelpers.WriteColored("\nHabit created!\n\n", ConsoleColor.Green);
        InputReader.AwaitAnyKeyPress();
    }

    private static void DeleteHabitMenu()
    {
        Console.Clear();
        
        List<Habit> habits = HabitsRepository.GetAll();
        
        if (habits.Count == 0)
        {
            Console.WriteLine("You have not created any habits yet.\n");
            InputReader.AwaitAnyKeyPress();
            return;
        }

        Console.Clear();
        Habit? habit = GetEntityChoice("Select a habit to delete:", habits, ListHabits);

        if (habit == null)
        {
            return;
        }
        
        HabitsRepository.Delete(habit.Id);

        ConsoleHelpers.WriteColored("\nHabit deleted.\n\n", ConsoleColor.Green);
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
        
        Habit? habit = GetEntityChoice("Select a habit to edit:", habits, ListHabits);

        if (habit == null)
        {
            return;
        }
        
        Console.Clear();
        
        ConsoleHelpers.WriteColored($"Editing habit \"{habit.Name}\"...\n\n", ConsoleColor.Yellow);
        
        string name = InputReader.GetString($"New habit name (press ENTER to keep current: {habit.Name}): ", habit.Name);
        string unitOfMeasurement = InputReader.GetString($"New unit of measurement (press ENTER to keep current: {habit.UnitOfMeasurement}): ", habit.UnitOfMeasurement);

        HabitsRepository.Update(habit.Id, name, unitOfMeasurement);
        
        ConsoleHelpers.WriteColored("\nHabit edited.\n\n", ConsoleColor.Green);
        InputReader.AwaitAnyKeyPress();
    }
    
    private static Habit CreateNewHabit()
    {
        string name = InputReader.GetString("Habit name: ");
        string unitOfMeasurement = InputReader.GetString("Unit of measurement: ");
        return HabitsRepository.Insert(name, unitOfMeasurement);
    }
    
    private static void ListAllHabitsWithOccurrences()
    {
        Console.Clear();
        List<Habit> habits = HabitsRepository.GetAll();
        List<Occurrence> occurrences = OccurrencesRepository.GetAll();

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

                List<Occurrence> currentHabitOccurrences = occurrences.FindAll(o => o.HabitId == habit.Id);

                if (currentHabitOccurrences.Count == 0)
                {
                    Console.WriteLine("└── There are no occurrences logged for this habit yet.");
                    continue;
                }
                
                for (int i = 0; i < currentHabitOccurrences.Count; i++)
                {
                    Occurrence occurrence = currentHabitOccurrences[i];
                    
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
    
    private static void ListOccurrences(List<OccurrenceWithHabit> occurrences)
    {
        foreach (OccurrenceWithHabit occurrence in occurrences)
        {
            ConsoleHelpers.WriteColored($"{occurrence.Id}.", ConsoleColor.Cyan);
            Console.Write(DateFormatter.FormatDateTimeString(occurrence.Date));
            Console.Write(" — ");
            Console.Write($"{occurrence.Quantity} {occurrence.UnitOfMeasurement}\n");
        }
    }
    
    private static void AddOccurrenceMenu()
    {
        Console.Clear();

        Habit? habit;
        
        List<Habit> habits = HabitsRepository.GetAll();
        
        if (habits.Count == 0)
        {
            Console.WriteLine("You have not created any habits yet.\n");
            habit = CreateNewHabit();
        }
        else
        {
            habit = GetEntityChoice("Select a habit:", habits, ListHabits);
            
            if (habit == null)
            {
                return;
            }
        }

        Console.Clear();
        ConsoleHelpers.WriteColored($"Adding an occurrence for \"{habit.Name}\"...\n\n", ConsoleColor.Yellow);
        
        string date = InputReader.GetDate(
            $"Provide a date ({DateFormatter.DateFormat} or press ENTER for today's date): ", 
            DateFormatter.DateFormat,
            DateFormatter.Culture
            );
        
        int quantity = InputReader.GetNumeric("Provide quantity: ");

        OccurrencesRepository.Insert(date, quantity, habit.Id);
        
        ConsoleHelpers.WriteColored("\nOccurrence added.\n\n", ConsoleColor.Green);
        InputReader.AwaitAnyKeyPress();
    }

    private static void EditOccurrenceMenu()
    {
        List<Habit> habits = HabitsRepository.GetAll();
        
        if (habits.Count == 0)
        {
            Console.WriteLine("You have not created any habits yet.\n");
            InputReader.AwaitAnyKeyPress();
            return;
        }

        Console.Clear();
        Habit? habit = GetEntityChoice("Select a habit:", habits, ListHabits);

        if (habit == null)
        {
            return;
        }
        
        Console.Clear();
        List<OccurrenceWithHabit> occurrences = OccurrencesRepository.GetAllByHabitId(habit.Id);
        
        ConsoleHelpers.WriteColored($"Editing an occurence for \"{habit.Name}\"...\n\n", ConsoleColor.Yellow);
        
        OccurrenceWithHabit? chosenOccurrence = GetEntityChoice("Select an occurrence to edit (press ENTER to go back to the Main Menu):", occurrences, ListOccurrences);

        if (chosenOccurrence == null)
        {
            return;
        }
        
        Console.Clear();
    
        ConsoleHelpers.WriteColored($"Editing Occurrence with ID {chosenOccurrence.Id} for \"{habit.Name}\"...\n\n", ConsoleColor.Yellow);
    
        string date = InputReader.GetDate(
            $"Provide new date (press ENTER to keep current: {DateFormatter.FormatDateTimeString(chosenOccurrence.Date)}): ", 
            DateFormatter.DateFormat, 
            DateFormatter.Culture, 
            chosenOccurrence.Date
            );
        
        int quantity = InputReader.GetNumeric(
            $"Provide new quantity (press ENTER to keep current: {chosenOccurrence.Quantity}): ",
            chosenOccurrence.Quantity
            );
        
        OccurrencesRepository.Update(chosenOccurrence.Id, date, quantity);
        
        ConsoleHelpers.WriteColored("\nOccurrence updated.\n\n", ConsoleColor.Green);
        InputReader.AwaitAnyKeyPress();
    }
    
    private static void DeleteOccurrence()
    {
        // TODO: Update this method
        Console.WriteLine();
        
        int occurrencesCount = OccurrencesRepository.GetAll().Count;
        
        if (occurrencesCount != 0)
        {
            int id = InputReader.GetNumeric("Provide ID of an occurence that you want to delete: ");

            OccurrencesRepository.Delete(id);

            ConsoleHelpers.WriteColored($"Occurrence with ID {id} was successfully deleted.\n", ConsoleColor.Green);
        }

        InputReader.AwaitAnyKeyPress();
    }
    
    private static T? GetEntityChoice<T>(string message, List<T> entities, Action<List<T>> displayDelegate) where T : class, IEntity
    {
        Console.WriteLine($"{message}\n");
        displayDelegate(entities);
        Console.WriteLine();
        
        while (true)
        {
            int? id = InputReader.GetNumericNullable("Your choice (press ENTER to go back to the Main Menu): ");
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