using Microsoft.Data.Sqlite;

namespace HabitTracker.dewsev;

public class HabitOccurrenceRepository
{
    private const string ConnectionString = "Data Source=HabitTracker.db";

    public HabitOccurrenceRepository()
    {
        Init();
    }

    private static void Init()
    {
        using (var connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();
            var createTableCommand = connection.CreateCommand();

            createTableCommand.CommandText =
                @"CREATE TABLE IF NOT EXISTS HabitOccurrences (
                    HabitOccurrenceID INTEGER PRIMARY KEY AUTOINCREMENT,
                    OccurrenceDate TEXT NOT NULL,
                    Quantity INTEGER NOT NULL
                    )";

            createTableCommand.ExecuteNonQuery();
            connection.Close();
        }
    }

    private void Insert(HabitOccurrence occurrence)
    {
        using (var connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = 
                $@"INSERT INTO HabitOccurrences (OccurrenceDate, Quantity)
                  VALUES ('{occurrence.Date}', {occurrence.Quantity})";
    
            insertCommand.ExecuteNonQuery();
            connection.Close();
        }
    }
}