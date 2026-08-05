using HabitTracker.dewsev.Entities;
using Microsoft.Data.Sqlite;

namespace HabitTracker.dewsev;

public class HabitsRepository
{
    private readonly string _connectionString; 
    
    public HabitsRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<Habit> GetAll()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();

        command.CommandText = "SELECT * FROM Habits";

        using var reader = command.ExecuteReader();

        List<Habit> habits = [];
        
        while (reader.Read())
        {
            habits.Add(new Habit
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                UnitOfMeasurement = reader.GetString(2)
            });
        }

        return habits;
    }

    public void Delete(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
            
        using var command = connection.CreateCommand();
        
        command.Parameters.Add("@id", SqliteType.Integer).Value = id;
        
        command.CommandText = "DELETE FROM Habits WHERE HabitID = @id";
        
        command.ExecuteNonQuery();
    }
    
    public Habit Insert(string name, string unitOfMeasurement)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = connection.CreateCommand();
        
        command.Parameters.Add("@name", SqliteType.Text).Value = name;
        command.Parameters.Add("@unitOfMeasurement", SqliteType.Text).Value = unitOfMeasurement;
        
        command.CommandText = @"INSERT INTO Habits (Name, UnitOfMeasurement) 
                                VALUES (@name, @unitOfMeasurement) 
                                RETURNING HabitID, Name, UnitOfMeasurement";

        using var reader = command.ExecuteReader();

        reader.Read();

        return new Habit
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            UnitOfMeasurement = reader.GetString(2)
        };
    }
    
    public void Update(int id, string name, string unitOfMeasurement)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        
        command.Parameters.Add("@name", SqliteType.Text).Value = name;
        command.Parameters.Add("@unitOfMeasurement", SqliteType.Text).Value = unitOfMeasurement;
        command.Parameters.Add("@id", SqliteType.Integer).Value = id;
        
        command.CommandText =
            "UPDATE Habits SET Name = @name, UnitOfMeasurement = @unitOfMeasurement WHERE HabitID = @id";

        command.ExecuteNonQuery();
    }
}