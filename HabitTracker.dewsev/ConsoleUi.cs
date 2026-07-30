namespace HabitTracker.dewsev;

public static class ConsoleUi
{
    public static void ShowHabitsWithOccurrences(List<Habit> habits, List<Occurrence> occurrences)
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
                    
                string prefixCharacter = i == currentHabitOccurrences.Count - 1 ? "└──" : "├──";
                Console.Write($"{prefixCharacter} {DateFormatter.FormatDateTimeString(occurrence.Date)}");
                Console.Write(" — ");
                Console.Write($"{occurrence.Quantity} {habit.UnitOfMeasurement}\n");
            }
        }
    }
    
    public static void ShowHabits(List<Habit> habits)
    {
        foreach (Habit habit in habits)
        {
            ConsoleHelpers.WriteColored($"{habit.Id}.", ConsoleColor.Cyan);
            Console.Write($"{habit.Name}\n");
        }
    }
    
    public static void ShowOccurrences(List<Occurrence> occurrences, string unitOfMeasurement)
    {
        foreach (Occurrence occurrence in occurrences)
        {
            ConsoleHelpers.WriteColored($"{occurrence.Id}.", ConsoleColor.Cyan);
            Console.Write(DateFormatter.FormatDateTimeString(occurrence.Date));
            Console.Write(" — ");
            Console.Write($"{occurrence.Quantity} {unitOfMeasurement}\n");
        }
    }
    
    public static void ShowEmptyHabitListMessage(bool awaitInput = true)
    {
        Console.WriteLine("You have not created any habits yet.\n");

        if (awaitInput)
        {
            InputReader.AwaitAnyKeyPress();    
        }
    }

    public static void ShowSuccess(string message)
    {
        ConsoleHelpers.WriteColored($"\n{message}\n\n", ConsoleColor.Green);
    }

    public static void ShowInfo(string message)
    {
        ConsoleHelpers.WriteColored($"{message}\n\n", ConsoleColor.Cyan);
    }
}