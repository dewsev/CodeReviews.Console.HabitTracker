using HabitTracker.dewsev.Entities;
using Microsoft.Data.Sqlite;

namespace HabitTracker.dewsev;

public class OccurrencesRepository
{
    private readonly string _connectionString;
    
    public OccurrencesRepository(string connectionString)
    {
        _connectionString = connectionString;
        CreateTable();
    }

    private void CreateTable()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var createTableCommand = connection.CreateCommand();
        
        createTableCommand.CommandText =
            @"CREATE TABLE IF NOT EXISTS Occurrences (
                    OccurrenceID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT NOT NULL,
                    Quantity INTEGER NOT NULL,
                    HabitID INTEGER NOT NULL,
                    FOREIGN KEY (HabitID) REFERENCES Habits(HabitID) ON DELETE CASCADE
                    )";

        createTableCommand.ExecuteNonQuery();
    }

    public List<Occurrence> GetAll()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
            
        using var command = connection.CreateCommand();
        
        command.CommandText = "SELECT * FROM Occurrences";

        List<Occurrence> occurrences = [];
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            occurrences.Add(new Occurrence
            {
                Id = reader.GetInt32(0),
                Date = DateTime.ParseExact(reader.GetString(1), DateFormatter.DateFormat, DateFormatter.Culture),
                Quantity = reader.GetInt32(2),
                HabitId = reader.GetInt32(3)
            });
        }
  
        return occurrences;
    }
    
    public List<Occurrence> GetAllByHabitId(int habitId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
            
        using var command = connection.CreateCommand();

        command.Parameters.Add("@habitId", SqliteType.Integer).Value = habitId;
        
        command.CommandText = "SELECT * FROM Occurrences WHERE HabitID = @habitId";

        List<Occurrence> occurrences = [];
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            occurrences.Add(new Occurrence
            {
                Id = reader.GetInt32(0),
                Date = DateTime.ParseExact(reader.GetString(1), DateFormatter.DateFormat, DateFormatter.Culture),
                Quantity = reader.GetInt32(2),
                HabitId = reader.GetInt32(3),
            });
        }
  
        return occurrences;
    }
  
    public void Delete(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
            
        using var command = connection.CreateCommand();
        
        command.Parameters.Add("@id", SqliteType.Integer).Value = id;
        
        command.CommandText = "DELETE FROM Occurrences WHERE OccurrenceID = @id";
        
        command.ExecuteNonQuery();
    }
    
    public void Insert(string date, int quantity, int habitId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        
        command.Parameters.Add("@date", SqliteType.Text).Value = date;
        command.Parameters.Add("@quantity", SqliteType.Integer).Value = quantity;
        command.Parameters.Add("@habitId", SqliteType.Integer).Value = habitId;
        
        command.CommandText = "INSERT INTO Occurrences (Date, Quantity, HabitID) VALUES (@date, @quantity, @habitId)";
    
        command.ExecuteNonQuery();
    }
    
    public void Update(int id, string date, int quantity)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        
        command.Parameters.Add("@id", SqliteType.Integer).Value = id;
        command.Parameters.Add("@date", SqliteType.Text).Value = date;
        command.Parameters.Add("@quantity", SqliteType.Integer).Value = quantity;
        
        command.CommandText = @"UPDATE Occurrences 
                                SET Date = @date, Quantity = @quantity
                                WHERE OccurrenceID = @id";
        
        command.ExecuteNonQuery();
    }
}