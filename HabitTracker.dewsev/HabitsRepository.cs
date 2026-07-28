using Microsoft.Data.Sqlite;

namespace HabitTracker.dewsev;

public class HabitsRepository
{
    private readonly string _connectionString; 
    
    public HabitsRepository(string connectionString)
    {
        _connectionString = connectionString;
        
        Init();
    }

    private void Init()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
            
        var createTableCommand = connection.CreateCommand();
        createTableCommand.CommandText =
            @"CREATE TABLE IF NOT EXISTS Habits (
                    HabitID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    UnitOfMeasurement TEXT NOT NULL
                    )";

        createTableCommand.ExecuteNonQuery();
    }
    
    public List<Habit> GetAll()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText = "SELECT * FROM Habits";

        var reader = command.ExecuteReader();

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

    public Habit? GetSingle(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.Parameters.Add("@id", SqliteType.Integer).Value = id;
        command.CommandText = "SELECT * FROM Habits WHERE HabitID = @id";

        var reader = command.ExecuteReader();

        if (!reader.HasRows)
        {
            return null;
        }
        
        reader.Read();

        return new Habit
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            UnitOfMeasurement = reader.GetString(2)
        };
    }
    
    public int Delete(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
            
        var command = connection.CreateCommand();
        
        command.Parameters.Add("@id", SqliteType.Integer).Value = id;
        command.CommandText = "DELETE FROM Habits WHERE HabitID = @id";
        
        return command.ExecuteNonQuery();
    }
    
    public int Insert(string name, string unitOfMeasurement)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        
        command.Parameters.Add("@name", SqliteType.Text).Value = name;
        command.Parameters.Add("@unitOfMeasurement", SqliteType.Text).Value = unitOfMeasurement;
        command.CommandText = "INSERT INTO Habits (Name, UnitOfMeasurement) VALUES (@name, @unitOfMeasurement) RETURNING HabitId";

        int insertedId = Convert.ToInt32(command.ExecuteScalar()!);
        return insertedId;
    }
    
    public bool Update(int id, string name, string unitOfMeasurement)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.Parameters.Add("@name", SqliteType.Text).Value = name;
        command.Parameters.Add("@unitOfMeasurement", SqliteType.Text).Value = unitOfMeasurement;
        command.Parameters.Add("@id", SqliteType.Integer).Value = id;
        command.CommandText =
            "UPDATE Habits SET Name = @name, UnitOfMeasurement = @unitOfMeasurement WHERE HabitID = @id";

        return command.ExecuteNonQuery() == 1;
    }
}