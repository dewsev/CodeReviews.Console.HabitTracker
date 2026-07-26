using System.Globalization;
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

    public List<HabitOccurrence> GetAll()
    {
        using (var connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();

            command.CommandText = "SELECT * FROM HabitOccurrences";

            List<HabitOccurrence> occurrences = [];
            SqliteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    occurrences.Add(new HabitOccurrence
                    {
                        Id = reader.GetInt32(0),
                        Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yyyy", new CultureInfo("en-US")),
                        Quantity = reader.GetInt32(2)
                    });
                }
            }
  
            return occurrences;
        }
    }
    
    public int Delete(int id)
    {
        using (var connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();
            
            var deleteCommand = connection.CreateCommand();

            deleteCommand.CommandText = 
                $@"DELETE FROM HabitOccurrences
                  WHERE HabitOccurrenceID = {id}";

            int deletedCount = deleteCommand.ExecuteNonQuery();
            
            connection.Close();

            return deletedCount;
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