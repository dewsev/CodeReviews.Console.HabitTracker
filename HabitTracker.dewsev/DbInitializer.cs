using Microsoft.Data.Sqlite;

namespace HabitTracker.dewsev;

public class DbInitializer
{
    private const string InitialSeedKey = "seed_v1";
    private readonly SqliteConnection _connection;
    private readonly SqliteTransaction _transaction;

    public DbInitializer(string connectionString)
    {
        _connection = new SqliteConnection(connectionString);
        _connection.Open();
        _transaction = _connection.BeginTransaction();
    }
    
    public void Initialize()
    {
        using (_connection)
        {
            try
            {
                CreateTables();

                if (NeedsSeeding())
                {
                    InsertSeed();
                    InsertHabits();
                    InsertOccurrences();
                }
            
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
        }
    }

    private void CreateTables()
    {
        using var command = _connection.CreateCommand();
        command.Transaction = _transaction;
        
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

    private bool NeedsSeeding()
    {
        using var command = _connection.CreateCommand();
        command.Transaction = _transaction;
        
        command.Parameters.Add("@key", SqliteType.Text).Value = InitialSeedKey;
        command.CommandText = "SELECT 1 FROM SeedState WHERE Key = @key LIMIT 1";

       return command.ExecuteScalar() == null;
    }

    private void InsertSeed()
    {
        using var command = _connection.CreateCommand();
        command.Transaction = _transaction;

        command.Parameters.Add("@key", SqliteType.Text).Value = InitialSeedKey;
        command.Parameters.Add("@appliedAt", SqliteType.Text).Value = DateParser.GetDateTimeString(DateTime.Now);
        command.CommandText = "INSERT INTO SeedState (Key, AppliedAt) VALUES (@key, @appliedAt);";

        command.ExecuteNonQuery();
    }

    private void InsertHabits()
    {
        using var command = _connection.CreateCommand();
        command.Transaction = _transaction;
        
        command.CommandText =
            @"INSERT INTO Habits (HabitID, Name, UnitOfMeasurement) VALUES
              (1, 'Exercising', 'minutes'),
              (2, 'Programming', 'minutes'),
              (3, 'Reading', 'minutes'),
              (4, 'Sleeping', 'hours'),
              (5, 'Water drinking', 'cups')";

        command.ExecuteNonQuery();
    }

    private void InsertOccurrences()
    {
        Random random = new Random();
        int count = 100;

        using var command = _connection.CreateCommand();
        command.Transaction = _transaction;
        
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