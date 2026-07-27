using System.Globalization;
using Microsoft.Data.Sqlite;

namespace HabitTracker.dewsev;

public class HabitOccurrencesRepository
{
    private readonly string _connectionString;
    private readonly string _dateFormat;
    private readonly CultureInfo _culture;
    
    public HabitOccurrencesRepository(string connectionString, string dateFormat, CultureInfo culture)
    {
        _connectionString = connectionString;
        _dateFormat = dateFormat;
        _culture = culture;
        
        Init();
    }

    private void Init()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var createTableCommand = connection.CreateCommand();
        createTableCommand.CommandText =
            @"CREATE TABLE IF NOT EXISTS HabitOccurrences (
                    HabitOccurrenceID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT NOT NULL,
                    Quantity INTEGER NOT NULL,
                    HabitID INTEGER NOT NULL,
                    FOREIGN KEY (HabitID) REFERENCES Habits(HabitID) ON DELETE CASCADE
                    )";

        createTableCommand.ExecuteNonQuery();
    }

    public List<HabitOccurrence> GetAll()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
            
        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM HabitOccurrences";

        List<HabitOccurrence> occurrences = [];
        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            occurrences.Add(new HabitOccurrence
            {
                Id = reader.GetInt32(0),
                Date = DateTime.ParseExact(reader.GetString(1), _dateFormat, _culture),
                Quantity = reader.GetInt32(2)
            });
        }
  
        return occurrences;
    }
    
    public int Delete(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
            
        var command = connection.CreateCommand();
        
        command.Parameters.Add("@id", SqliteType.Integer).Value = id;
        command.CommandText = "DELETE FROM HabitOccurrences WHERE HabitOccurrenceID = @id";
        
        return command.ExecuteNonQuery();
    }
    
    public void Insert(string date, int quantity)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        
        command.Parameters.Add("@date", SqliteType.Text).Value = date;
        command.Parameters.Add("@quantity", SqliteType.Integer).Value = quantity;
        command.CommandText = "INSERT INTO HabitOccurrences (Date, Quantity) VALUES (@date, @quantity)";
    
        command.ExecuteNonQuery();
    }

    public HabitOccurrence? GetSingle(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        
        command.Parameters.Add("@id", SqliteType.Integer).Value = id;
        command.CommandText = "SELECT * FROM HabitOccurrences WHERE HabitOccurrenceID = @id";

        var reader = command.ExecuteReader();

        if (!reader.HasRows)
        {
            return null;
        }

        reader.Read();
            
        return new HabitOccurrence
        {
            Id = reader.GetInt32(0),
            Date = DateTime.ParseExact(reader.GetString(1), _dateFormat, _culture),
            Quantity = reader.GetInt32(2)
        };
    }
    
    public void Update(int id, string date, int quantity)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        
        command.Parameters.Add("@id", SqliteType.Integer).Value = id;
        command.Parameters.Add("@date", SqliteType.Text).Value = date;
        command.Parameters.Add("@quantity", SqliteType.Integer).Value = quantity;
        command.CommandText = @"UPDATE HabitOccurrences 
                                SET Date = @date, Quantity = @quantity
                                WHERE HabitOccurrenceID = @id";
    
        command.ExecuteNonQuery();
    }
}