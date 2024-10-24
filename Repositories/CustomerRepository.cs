using System.Data.SqlClient;
using ChinookApp.Models;

namespace ChinookSuperheroes.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly SqlConnection _connection;
        private static bool _databaseInitialized = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerRepository"/> class.
        /// </summary>
        /// <param name="connection">The SQL connection to use for database operations.</param>
        public CustomerRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Retrieves all customers from the database asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable list of customers.</returns>
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

        /// <summary>
        /// Retrieves a customer by their ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the customer to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the customer if found; otherwise, null.</returns>
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

        /// <summary>
        /// Retrieves a customer by their name asynchronously.
        /// </summary>
        /// <param name="query">The name query to search for.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the customer if found; otherwise, null.</returns>
        public async Task<Customer?> GetCustomerByNameAsync(string query)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM Customer WHERE FirstName LIKE @Query OR LastName LIKE @Query";
            command.Parameters.AddWithValue("@Query", $"%{query}%");

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

        /// <summary>
        /// Adds a new customer to the database asynchronously.
        /// </summary>
        /// <param name="customer">The customer to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
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

        /// <summary>
        /// Retrieves a paginated list of customers from the database asynchronously.
        /// </summary>
        /// <param name="limit">The maximum number of customers to retrieve.</param>
        /// <param name="offset">The number of customers to skip before starting to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable list of customers.</returns>
        public async Task<IEnumerable<Customer>> GetCustomersPagedAsync(int limit, int offset)
        {
            var customers = new List<Customer>();
            var command = _connection.CreateCommand();
            command.CommandText = @"
                SELECT * FROM Customer
                ORDER BY CustomerId
                OFFSET @Offset ROWS
                FETCH NEXT @Limit ROWS ONLY";
            command.Parameters.AddWithValue("@Limit", limit);
            command.Parameters.AddWithValue("@Offset", offset);

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

        /// <summary>
        /// Retrieves a customer by their email address asynchronously.
        /// </summary>
        /// <param name="email">The email address of the customer to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the customer if found; otherwise, null.</returns>
        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            Customer? customer = null;
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM Customer WHERE Email = @Email";
            command.Parameters.AddWithValue("@Email", email);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                customer = new Customer
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

            return customer;
        }

        /// <summary>
        /// Updates an existing customer in the database asynchronously.
        /// </summary>
        /// <param name="customer">The customer with updated information.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
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

        /// <summary>
        /// Deletes a customer from the database asynchronously.
        /// </summary>
        /// <param name="id">The ID of the customer to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task DeleteCustomerAsync(int id)
        {
            var command = new SqlCommand("DELETE FROM Customer WHERE CustomerId = @Id", _connection);
            command.Parameters.AddWithValue("@Id", id);

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Initializes the database asynchronously, creating it if it does not exist.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
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

        /// <summary>
        /// Checks if the database exists asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the database exists.</returns>
        private async Task<bool> CheckDatabaseExistsAsync()
        {
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM sys.databases WHERE name = 'ChinookDB'";
            command.Parameters.AddWithValue("@ChinookDB", _connection.Database);
            
            int count = (int)(await command.ExecuteScalarAsync() ?? 0);
            return count > 0;
        }

        /// <summary>
        /// Creates the database asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task CreateDatabaseAsync()
        {
            var command = _connection.CreateCommand();
            command.CommandText = $"CREATE DATABASE {_connection.Database}";
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Creates the necessary tables in the database asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
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

        /// <summary>
        /// Retrieves a paginated list of customers from the database asynchronously.
        /// </summary>
        /// <param name="limit">The maximum number of customers to retrieve.</param>
        /// <param name="offset">The number of customers to skip before starting to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of customers.</returns>
        
        /// <summary>
        /// Retrieves the count of customers by country in descending order asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of customer countries with their respective customer counts.</returns>
        public async Task<IEnumerable<CustomerCountry>> GetCustomerCountByCountryDescAsync()
        {
            var customerCountries = new List<CustomerCountry>();
            var command = _connection.CreateCommand();
            command.CommandText = @"
                SELECT Country, COUNT(*) AS CustomerCount
                FROM Customer
                GROUP BY Country
                ORDER BY CustomerCount DESC";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                customerCountries.Add(new CustomerCountry
                {
                    Name = reader.GetString(reader.GetOrdinal("Country")),
                    CustomerCount = reader.GetInt32(reader.GetOrdinal("CustomerCount"))
                });
            }

            return customerCountries;
        }

        /// <summary>
        /// Retrieves the high spenders in descending order asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of customer spenders with their respective total spent amounts.</returns>
        public async Task<IEnumerable<Customer>> GetHighSpendersDescendingAsync()
        {
            var highSpenders = new List<Customer>();
            var command = _connection.CreateCommand();
            command.CommandText = @"
                SELECT CustomerId, FirstName, LastName, Email, Phone, Country, PostalCode, SUM(Total) AS TotalSpent
                FROM Invoice
                INNER JOIN Customer ON Invoice.CustomerId = Customer.CustomerId
                GROUP BY CustomerId, FirstName, LastName, Email, Phone, Country, PostalCode
                ORDER BY TotalSpent DESC";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                highSpenders.Add(new Customer
                {
                    Id = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                    Country = reader.GetString(reader.GetOrdinal("Country")),
                    PostalCode = reader.GetString(reader.GetOrdinal("PostalCode")),
                    // Assuming you have a TotalSpent property in Customer class
                    TotalSpent = reader.GetDecimal(reader.GetOrdinal("TotalSpent"))
                });
            }

            return highSpenders;
        }

        /// <summary>
        /// Retrieves the most popular genres for a given customer asynchronously.
        /// </summary>
        /// <param name="customerId">The ID of the customer to retrieve the most popular genres for.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of customer genres with their respective track counts.</returns>
        public async Task<IEnumerable<CustomerGenre>> GetMostPopularGenresByCustomerIdAsync(int customerId)
        {
            var popularGenres = new List<CustomerGenre>();
            var command = _connection.CreateCommand();
            command.CommandText = @"
                SELECT Genre.Name, COUNT(*) AS TrackCount
                FROM InvoiceLine
                INNER JOIN Invoice ON InvoiceLine.InvoiceId = Invoice.InvoiceId
                INNER JOIN Track ON InvoiceLine.TrackId = Track.TrackId
                INNER JOIN Genre ON Track.GenreId = Genre.GenreId
                WHERE Invoice.CustomerId = @CustomerId
                GROUP BY Genre.Name
                HAVING COUNT(*) = (
                    SELECT MAX(TrackCount)
                    FROM (
                        SELECT COUNT(*) AS TrackCount
                        FROM InvoiceLine
                        INNER JOIN Invoice ON InvoiceLine.InvoiceId = Invoice.InvoiceId
                        INNER JOIN Track ON InvoiceLine.TrackId = Track.TrackId
                        INNER JOIN Genre ON Track.GenreId = Genre.GenreId
                        WHERE Invoice.CustomerId = @CustomerId
                        GROUP BY Genre.Name
                    ) AS GenreCounts
                )";

            command.Parameters.AddWithValue("@CustomerId", customerId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                popularGenres.Add(new CustomerGenre
                {
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    TrackCount = reader.GetInt32(reader.GetOrdinal("TrackCount"))
                });
            }

            return popularGenres;
        }
    }
}
