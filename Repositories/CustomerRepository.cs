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

        public async Task<Customer> GetCustomerByIdAsync(int id)
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
    }
}
