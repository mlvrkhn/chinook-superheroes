using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

            _databaseInitialized = true;

            try
            {
                // First, ensure we're connected to a default database (like 'master')
                var command = new SqlCommand("SELECT DB_ID('Superheroes')", _connection);
                var result = await command.ExecuteScalarAsync();

                if (result == DBNull.Value)
                {
                    // Database doesn't exist, so create it
                    command = new SqlCommand("CREATE DATABASE SuperheroesDB", _connection);
                    await command.ExecuteNonQueryAsync();
                    Console.WriteLine("SuperheroesDB created successfully.");
                }

                // Switch to the SuperheroesDB
                _connection.ChangeDatabase("SuperheroesDB");

                // Now create the Customer table if it doesn't exist
                command = new SqlCommand(@"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Customer' and xtype='U')
                    BEGIN
                        CREATE TABLE Customer (
                            CustomerId INT PRIMARY KEY IDENTITY(1,1),
                            FirstName NVARCHAR(50),
                            LastName NVARCHAR(50)
                        )
                        PRINT 'Customer table created successfully.'
                    END
                    ELSE
                    BEGIN
                        PRINT 'Customer table already exists.'
                    END", _connection);
                await command.ExecuteNonQueryAsync();

                Console.WriteLine("Database initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in InitializeDatabaseAsync: {ex.Message}");
                _databaseInitialized = false; // Reset if initialization fails
                throw;
            }
        }
    }
}
