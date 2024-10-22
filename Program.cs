using System.Data.SqlClient;
using ChinookSuperheroes;
using ChinookSuperheroes.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            // Build configuration
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Setup dependency injection
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<ICustomerRepository, CustomerRepository>();
            services.AddSingleton<SqlConnection>(_ => new SqlConnection(GetConnectionString(configuration)));
            var serviceProvider = services.BuildServiceProvider();

            // Get the SqlConnection from the service provider
            var connection = serviceProvider.GetRequiredService<SqlConnection>();

            // Open the connection
            await connection.OpenAsync();

            // Create an instance of the Application class
            var app = new Application(connection.ConnectionString, serviceProvider.GetRequiredService<ICustomerRepository>());

            // Run the application
            await app.RunAsync(args);

            // Close the connection when done
            await connection.CloseAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves the connection string from the configuration.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The connection string for the database.</returns>
    static string GetConnectionString(IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("DefaultConnection string is not found in the configuration.");
        }
        return connectionString;
    }
}
