using HabitTracker.dewsev.Entities;
using Microsoft.Data.Sqlite;

namespace HabitTracker.dewsev;

public class DbInitializer
{
    public void Initialize(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();
        try
        {
            CreateTables(connection, transaction);

            const string seedKey = "seed_v1";
            bool needsSeeding = NeedsSeeding(connection, transaction, seedKey);

            if (needsSeeding)
            {
                InsertSeed(connection, transaction, seedKey);
                InsertHabits(connection, transaction);
                InsertOccurrences(connection, transaction);
            }
            
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static void CreateTables(SqliteConnection connection, SqliteTransaction transaction)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        
        command.CommandText =
            @"CREATE TABLE IF NOT EXISTS Habits (
                HabitID INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                UnitOfMeasurement TEXT NOT NULL);

              CREATE TABLE IF NOT EXISTS Occurrences (
                OccurrenceID INTEGER PRIMARY KEY AUTOINCREMENT,
                Date TEXT NOT NULL,
                Quantity INTEGER NOT NULL,
                HabitID INTEGER NOT NULL,
                FOREIGN KEY (HabitID) REFERENCES Habits(HabitID) ON DELETE CASCADE);

              CREATE TABLE IF NOT EXISTS SeedState (
                Key TEXT PRIMARY KEY,
                AppliedAt TEXT NOT NULL);";

        command.ExecuteNonQuery();
    }

    private static bool NeedsSeeding(SqliteConnection connection, SqliteTransaction transaction, string seedKey)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        
        command.Parameters.Add("@key", SqliteType.Text).Value = seedKey;
        command.CommandText = "SELECT 1 FROM SeedState WHERE Key = @key LIMIT 1";

       return command.ExecuteScalar() == null;
    }

    private static void InsertSeed(SqliteConnection connection, SqliteTransaction transaction, string key)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;

        command.Parameters.Add("@key", SqliteType.Text).Value = key;
        command.Parameters.Add("@appliedAt", SqliteType.Text).Value = DateParser.GetDateTimeString(DateTime.Now);
        command.CommandText = "INSERT INTO SeedState (Key, AppliedAt) VALUES (@key, @appliedAt);";

        command.ExecuteNonQuery();
    }

    private static void InsertHabits(SqliteConnection connection, SqliteTransaction transaction)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        
        command.CommandText =
            @"INSERT INTO Habits (HabitID, Name, UnitOfMeasurement) VALUES
              (1, 'Exercising', 'minutes'),
              (2, 'Programming', 'minutes'),
              (3, 'Reading', 'minutes'),
              (4, 'Sleeping', 'hours'),
              (5, 'Water drinking', 'cups')";

        command.ExecuteNonQuery();
    }

    private static void InsertOccurrences(SqliteConnection connection, SqliteTransaction transaction)
    {
        Random random = new Random();
        int count = 100;

        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        
        string[] sqlValues = new string[count];
        
        for (int i = 1; i <= count; i++)
        {
            int day = random.Next(1, 29);
            int month = random.Next(1, 12);
            int year = random.Next(2024, 2027);

            string date = DateParser.GetDateTimeString(new DateTime(year, month, day));
            int habitId = random.Next(1, 6);
            int quantity = random.Next(5, 13);
            
            sqlValues[i - 1] = $"(@id{i}, @date{i}, @quantity{i}, @habitId{i})";
            command.Parameters.AddWithValue($"@id{i}", i);
            command.Parameters.AddWithValue($"@date{i}", date);
            command.Parameters.AddWithValue($"@quantity{i}", quantity);
            command.Parameters.AddWithValue($"@habitId{i}", habitId);
        }
        
        command.CommandText = $"INSERT INTO Occurrences VALUES {string.Join(",", sqlValues)}";
        
        command.ExecuteNonQuery();
    }
}