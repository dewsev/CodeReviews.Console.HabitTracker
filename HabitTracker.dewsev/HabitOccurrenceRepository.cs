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

    public void Insert(string date, int quantity)
    {
        using (var connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = 
                $@"INSERT INTO HabitOccurrences (OccurrenceDate, Quantity)
                  VALUES ('{date}', {quantity})";
    
            insertCommand.ExecuteNonQuery();
            connection.Close();
        }
    }
}