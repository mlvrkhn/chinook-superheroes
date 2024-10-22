using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChinookApp.Models;

namespace ChinookSuperheroes.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly SqlConnection _connection;
        private static bool _databaseInitialized = false;

        public CustomerRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            var customers = new List<Customer>();
            var command = new SqlCommand("SELECT * FROM Customer", _connection);
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                customers.Add(new Customer
                {
                    Id = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    // Add other properties as needed
                });
            }

            return customers;
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM Customer WHERE CustomerId = @CustomerId";
            command.Parameters.AddWithValue("@CustomerId", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Customer
                {
                    Id = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    // Add other properties as needed
                };
            }

            return null;
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            var command = new SqlCommand("INSERT INTO Customer (FirstName, LastName) VALUES (@FirstName, @LastName)", _connection);
            command.Parameters.AddWithValue("@FirstName", customer.FirstName);
            command.Parameters.AddWithValue("@LastName", customer.LastName);
            // Add other parameters as needed

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            var command = _connection.CreateCommand();
            command.CommandText = @"
                UPDATE Customer 
                SET FirstName = @FirstName, LastName = @LastName, /* other fields... */
                WHERE CustomerId = @CustomerId";
            
            command.Parameters.AddWithValue("@CustomerId", customer.Id);
            command.Parameters.AddWithValue("@FirstName", customer.FirstName);
            command.Parameters.AddWithValue("@LastName", customer.LastName);
            // Add other parameters as needed

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var command = new SqlCommand("DELETE FROM Customer WHERE CustomerId = @Id", _connection);
            command.Parameters.AddWithValue("@Id", id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task InitializeDatabaseAsync()
        {
            if (_databaseInitialized) return;

            try
            {
                // Check if the database exists
                bool dbExists = await CheckDatabaseExistsAsync();

                if (!dbExists)
                {
                    // Create the database if it doesn't exist
                    await CreateDatabaseAsync();
                    Console.WriteLine("Database created successfully.");
                }
                else
                {
                    Console.WriteLine("Database already exists. Skipping creation.");
                }

                // Proceed with table creation and other initialization steps
                await CreateTablesAsync();
                
                _databaseInitialized = true;
                Console.WriteLine("Database initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in InitializeDatabaseAsync: {ex.Message}");
                throw;
            }
        }

        private async Task<bool> CheckDatabaseExistsAsync()
        {
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM sys.databases WHERE name = @dbName";
            command.Parameters.AddWithValue("@dbName", _connection.Database);
            
            int count = (int)(await command.ExecuteScalarAsync() ?? 0);
            return count > 0;
        }

        private async Task CreateDatabaseAsync()
        {
            var command = _connection.CreateCommand();
            command.CommandText = $"CREATE DATABASE {_connection.Database}";
            await command.ExecuteNonQueryAsync();
        }

        private async Task CreateTablesAsync()
        {
            string[] sqlFiles = Directory.GetFiles("SQL_Commands", "*.sql");

            foreach (string sqlFile in sqlFiles)
            {
                string sqlContent = await File.ReadAllTextAsync(sqlFile);
                string[] sqlCommands = sqlContent.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string sqlCommand in sqlCommands)
                {
                    if (string.IsNullOrWhiteSpace(sqlCommand))
                        continue;

                    using var command = _connection.CreateCommand();
                    command.CommandText = sqlCommand;
                    try
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine($"Error executing SQL from {Path.GetFileName(sqlFile)}: {ex.Message}");
                        // Optionally, you might want to throw the exception here if you want to stop the process
                        // throw;
                    }
                }
            }

            Console.WriteLine("All tables created successfully.");
        }
    }
}
