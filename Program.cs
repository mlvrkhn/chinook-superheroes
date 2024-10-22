// See https://aka.ms/new-console-template for more information
using System.Data.SqlClient;
using System.Text.Json;
class Program
{
    /// <summary>
    /// The entry point of the application.
    /// </summary>
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to SuperheroesDB Console App!");
        
        string connectionString = GetConnectionString();
        CreateDatabase(connectionString);
        CreateTables(connectionString);
        
        Console.WriteLine("Database and tables created successfully!");
    }

    /// <summary>
    /// Retrieves the connection string from the configuration.
    /// </summary>
    /// <returns>The connection string for the database.</returns>
    static string GetConnectionString()
    {
        // Read connection string from appsettings.json
        string json = File.ReadAllText("appsettings.json");
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        JsonElement connectionStrings = root.GetProperty("ConnectionStrings");
        return connectionStrings.GetProperty("DefaultConnection").GetString();
    }

    /// <summary>
    /// Creates the SuperheroesDb database if it doesn't exist.
    /// </summary>
    /// <param name="connectionString">The connection string to the server.</param>
    static void CreateDatabase(string connectionString)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string sql = "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'SuperheroesDb') CREATE DATABASE SuperheroesDb;";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// Creates the necessary tables in the SuperheroesDb database.
    /// </summary>
    /// <param name="connectionString">The connection string to the database.</param>
    static void CreateTables(string connectionString)
    {
        // TODO: Implement table creation logic
        Console.WriteLine("Creating tables...");
    }
}
