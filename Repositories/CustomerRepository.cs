using System.Data.SqlClient;
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
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                    Country = reader.GetString(reader.GetOrdinal("Country")),
                    PostalCode = reader.GetString(reader.GetOrdinal("PostalCode")),
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
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                    Country = reader.GetString(reader.GetOrdinal("Country")),
                    PostalCode = reader.GetString(reader.GetOrdinal("PostalCode")),
                };
            }

            return null;
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            try
            {
                var command = new SqlCommand("INSERT INTO Customer (FirstName, LastName, Email, Phone, Country, PostalCode) VALUES (@FirstName, @LastName, @Email, @Phone, @Country, @PostalCode)", _connection);
                command.Parameters.AddWithValue("@FirstName", customer.FirstName);
                command.Parameters.AddWithValue("@LastName", customer.LastName);
                command.Parameters.AddWithValue("@Email", customer.Email);
                command.Parameters.AddWithValue("@Phone", customer.Phone);
                command.Parameters.AddWithValue("@Country", customer.Country);
                command.Parameters.AddWithValue("@PostalCode", customer.PostalCode);

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error adding customer: {ex.Message}", ex);
            }
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            try
            {
                var command = _connection.CreateCommand();
                command.CommandText = @"
                    UPDATE Customer 
                    SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Phone = @Phone, Country = @Country, PostalCode = @PostalCode
                    WHERE CustomerId = @CustomerId";
                
                command.Parameters.AddWithValue("@CustomerId", customer.Id);
                command.Parameters.AddWithValue("@FirstName", customer.FirstName);
                command.Parameters.AddWithValue("@LastName", customer.LastName);
                command.Parameters.AddWithValue("@Email", customer.Email);
                command.Parameters.AddWithValue("@Phone", customer.Phone);
                command.Parameters.AddWithValue("@Country", customer.Country);
                command.Parameters.AddWithValue("@PostalCode", customer.PostalCode);

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error updating customer: {ex.Message}", ex);
            }
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
            var command = _connection.CreateCommand();
            command.CommandText = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Customer' AND xtype='U')
                BEGIN
                    CREATE TABLE Customer (
                        CustomerId INT PRIMARY KEY IDENTITY(1,1),
                        FirstName NVARCHAR(40) NOT NULL,
                        LastName NVARCHAR(20) NOT NULL,
                        Email NVARCHAR(60) NOT NULL,
                        Phone NVARCHAR(24),
                        Country NVARCHAR(40),
                        PostalCode NVARCHAR(10)
                    )
                END";

            try
            {
                await command.ExecuteNonQueryAsync();
                Console.WriteLine("Customer table created successfully or already exists.");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error creating Customer table: {ex.Message}");
                throw;
            }
        }
    }
}
