using HabitTracker.dewsev.Entities;

namespace HabitTracker.dewsev;

public static class ConsoleRenderer
{
    public static void RenderHabitListWithOccurrences(List<Habit> habits, List<Occurrence> allOccurrences)
    {
        foreach (Habit habit in habits)
        {
            ConsoleHelpers.WriteColored($"{habit.Id}.", ConsoleColor.Cyan);
            Console.Write($"{habit.Name}\n");

            List<Occurrence> currentHabitOccurrences = allOccurrences.FindAll(o => o.HabitId == habit.Id);
            RenderOccurrenceListForHabit(currentHabitOccurrences, habit.UnitOfMeasurement);
            Console.WriteLine();
        }
    }
    
    public static void RenderHabitWithOccurrences(Habit habit, List<Occurrence> occurrences)
    {
        ConsoleHelpers.WriteColored($"{habit.Name}\n", ConsoleColor.Cyan);
        RenderOccurrenceListForHabit(occurrences, habit.UnitOfMeasurement);
    }

    private static void RenderOccurrenceListForHabit(List<Occurrence> occurrences, string unitOfMeasurement)
    {
        if (occurrences.Count == 0)
        {
            Console.WriteLine("└── There are no occurrences logged for this habit yet.");
        }
        
        for (int i = 0; i < occurrences.Count; i++)
        {
            Occurrence occurrence = occurrences[i];
                
            string prefixCharacter = i == occurrences.Count - 1 ? "└──" : "├──";
            Console.Write($"{prefixCharacter} {DateParser.GetDateTimeString(occurrence.Date)}");
            Console.Write(" — ");
            Console.Write($"{occurrence.Quantity} {unitOfMeasurement}\n");
        }
    }
    
    public static void RenderHabitList(List<Habit> habits)
    {
        foreach (Habit habit in habits)
        {
            ConsoleHelpers.WriteColored($"{habit.Id}.", ConsoleColor.Cyan);
            Console.Write($"{habit.Name}\n");
        }
    }
    
    public static void RenderOccurrenceListWithIds(List<Occurrence> occurrences, string unitOfMeasurement)
    {
        foreach (Occurrence occurrence in occurrences)
        {
            ConsoleHelpers.WriteColored($"{occurrence.Id}.", ConsoleColor.Cyan);
            Console.Write(DateParser.GetDateTimeString(occurrence.Date));
            Console.Write(" — ");
            Console.Write($"{occurrence.Quantity} {unitOfMeasurement}\n");
        }
    }
    
    public static void RenderEmptyHabitListMessage()
    {
        Console.WriteLine("You have not created any habits yet.\n");
    }

    public static void RenderEmptyOccurrenceListMessage()
    {
        Console.WriteLine("This habit doesn't have any occurrences yet.\n");
    }
}