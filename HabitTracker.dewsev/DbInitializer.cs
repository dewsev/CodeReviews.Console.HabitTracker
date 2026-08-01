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
            InsertSeedData(connection, transaction);
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

    private static void InsertSeedData(SqliteConnection connection, SqliteTransaction transaction)
    {
        // TODO: This method is doing too much work
        const string seedKey = "seed_v1";

        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        
        command.Parameters.Add("@key", SqliteType.Text).Value = seedKey;
        command.CommandText = "SELECT 1 FROM SeedState WHERE Key = @key LIMIT 1";

        bool alreadySeeded = command.ExecuteScalar() != null;
        if (alreadySeeded) return;
        
        Console.WriteLine("SEEDING....");
        InsertSeed(connection, transaction, seedKey);
        InsertHabits(connection, transaction);
        InsertOccurrences(connection, transaction);
    }

    private static void InsertSeed(SqliteConnection connection, SqliteTransaction transaction, string key)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;

        command.Parameters.Add("@key", SqliteType.Text).Value = key;
        command.Parameters.Add("@appliedAt", SqliteType.Text).Value = DateFormatter.FormatDateTime(DateTime.Now);
        command.CommandText = "INSERT INTO SeedState (Key, AppliedAt) VALUES (@key, @appliedAt);";

        command.ExecuteNonQuery();
    }

    private static void InsertHabits(SqliteConnection connection, SqliteTransaction transaction)
    {
        var habits = new[]
        {
            new Habit{ Id = 1, Name = "Exercising", UnitOfMeasurement = "minutes" },
            new Habit{ Id = 2, Name = "Programming", UnitOfMeasurement = "minutes" },
            new Habit{ Id = 3, Name = "Reading", UnitOfMeasurement = "minutes" },
            new Habit{ Id = 4, Name = "Sleeping good", UnitOfMeasurement = "hours" },
            new Habit{ Id = 5, Name = "Water drinking", UnitOfMeasurement = "cups" },
        };
        
        using var command = connection.CreateCommand();
        command.Transaction = transaction;

        List<string> sqlValues = [];
        for (int i = 0; i < habits.Length; i++)
        {
            sqlValues.Add($"(@id{i}, @name{i}, @unit{i})");
            command.Parameters.AddWithValue($"@id{i}", habits[i].Id);
            command.Parameters.AddWithValue($"@name{i}", habits[i].Name);
            command.Parameters.AddWithValue($"@unit{i}", habits[i].UnitOfMeasurement);
        }
        
        command.CommandText =
            $"INSERT INTO Habits (HabitID, Name, UnitOfMeasurement) VALUES {string.Join(",", sqlValues)};";

        command.ExecuteNonQuery();
    }

    private static void InsertOccurrences(SqliteConnection connection, SqliteTransaction transaction)
    {
        // Generate random occurrences
    }
}