using ChinookApp.Models;
using ChinookSuperheroes.Repositories;
using static ChinookApp.Helpers.Helpers;
using System.Linq;

namespace ChinookSuperheroes
{
    public class Application
    {
        private readonly ICustomerRepository _customerRepository;
        private bool _isInitialized = false;

        public Application(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        /// <summary>
        /// Runs the application asynchronously based on the provided command-line arguments.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        public async Task RunAsync(string[] args)
        {
            if (!_isInitialized)
            {
                await InitializeDatabaseAsync();
                _isInitialized = true;
            }

            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            switch (args[0].ToLower())
            {
                case "init":
                    await InitializeDatabaseAsync();
                    break;
                case "add":
                    await AddCustomerAsync();
                    break;
                case "update":
                    await UpdateCustomerAsync();
                    break;
                case "search":
                    await GetCustomerByNameAsync();
                    break;
                case "delete":
                    await DeleteCustomerAsync();
                    break;
                case "list":
                    await GetAllCustomersAsync();
                    break;
                case "getbyid":
                    await GetCustomerByIdAsync();
                    break;
                case "getpaged":
                    await GetCustomersPagedAsync();
                    break;
                case "countbycountry":
                    await GetCustomerCountByCountryDescAsync();
                    break;
                case "highspenders":
                    await GetHighSpendersDescendingAsync();
                    break;
                case "populargenres":
                    await GetMostPopularGenresByCustomerIdAsync();
                    break;
                default:
                    Console.WriteLine("Unknown command. Use 'init', 'add', 'update', 'delete', or 'list'.");
                    ShowHelp();
                    break;
            }
        }

        /// <summary>
        /// Adds a new customer to the database asynchronously.
        /// </summary>
        private async Task AddCustomerAsync()
        {
            var newCustomer = new Customer
            {
                FirstName = "",
                LastName = "",
                Email = "",
                Phone = "",
                Country = "",
                PostalCode = ""
            };

            // First Name
            do
            {
                Console.Write("Enter customer's first name (required): ");
                newCustomer.FirstName = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(newCustomer.FirstName))
                {
                    Console.WriteLine("First name is required. Please enter a valid name.");
                }
            } while (string.IsNullOrWhiteSpace(newCustomer.FirstName));

            // Last Name
            do
            {
                Console.Write("Enter customer's last name (required): ");
                newCustomer.LastName = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(newCustomer.LastName))
                {
                    Console.WriteLine("Last name is required. Please enter a valid name.");
                }
            } while (string.IsNullOrWhiteSpace(newCustomer.LastName));

            // Email
            do
            {
                Console.Write("Enter customer's email (required): ");
                newCustomer.Email = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(newCustomer.Email))
                {
                    Console.WriteLine("Email is required. Please enter a valid email address.");
                }
                else if (!IsValidEmail(newCustomer.Email))
                {
                    Console.WriteLine("Invalid email format. Please enter a valid email address.");
                    newCustomer.Email = ""; // Reset to force re-entry
                }
            } while (string.IsNullOrWhiteSpace(newCustomer.Email) || !IsValidEmail(newCustomer.Email));

            // Optional fields
            Console.Write("Enter customer's phone: ");
            newCustomer.Phone = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Enter customer's country: ");
            newCustomer.Country = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Enter customer's postal code: ");
            newCustomer.PostalCode = Console.ReadLine()?.Trim() ?? "";

            try
            {
                await _customerRepository.AddCustomerAsync(newCustomer);
                Console.WriteLine("Customer added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding customer: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing customer in the database asynchronously.
        /// </summary>
        private async Task UpdateCustomerAsync()
        {
            Console.Write("Enter customer ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("Invalid customer ID.");
                return;
            }

            var existingCustomer = await _customerRepository.GetCustomerByIdAsync(customerId);
            if (existingCustomer == null)
            {
                Console.WriteLine("Customer not found in the database.");
                return;
            }

            Console.Write($"Enter new first name (current: {existingCustomer.FirstName}): ");
            existingCustomer.FirstName = Console.ReadLine()?.Trim() ?? existingCustomer.FirstName;

            Console.Write($"Enter new last name (current: {existingCustomer.LastName}): ");
            existingCustomer.LastName = Console.ReadLine()?.Trim() ?? existingCustomer.LastName;

            Console.Write($"Enter new email (current: {existingCustomer.Email}): ");
            string newEmail = Console.ReadLine()?.Trim() ?? existingCustomer.Email;
            while (!string.IsNullOrWhiteSpace(newEmail) && !IsValidEmail(newEmail))
            {
                Console.WriteLine("Invalid email format. Please enter a valid email address.");
                Console.Write($"Enter new email (current: {existingCustomer.Email}): ");
                newEmail = Console.ReadLine()?.Trim() ?? existingCustomer.Email;
            }
            existingCustomer.Email = newEmail;

            Console.Write($"Enter new phone (current: {existingCustomer.Phone}): ");
            existingCustomer.Phone = Console.ReadLine()?.Trim() ?? existingCustomer.Phone;

            Console.Write($"Enter new country (current: {existingCustomer.Country}): ");
            existingCustomer.Country = Console.ReadLine()?.Trim() ?? existingCustomer.Country;

            Console.Write($"Enter new postal code (current: {existingCustomer.PostalCode}): ");
            existingCustomer.PostalCode = Console.ReadLine()?.Trim() ?? existingCustomer.PostalCode;

            try
            {
                await _customerRepository.UpdateCustomerAsync(existingCustomer);
                Console.WriteLine("Customer updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating customer: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a customer from the database asynchronously.
        /// </summary>
        private async Task DeleteCustomerAsync()
        {
            Console.WriteLine("Deleting a customer...");
            Console.Write("Enter customer ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("Invalid customer ID.");
                return;
            }

            try
            {
                await _customerRepository.DeleteCustomerAsync(customerId);
                Console.WriteLine("Customer deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting customer: {ex.Message}");
            }
        }

        /// <summary>
        /// Initializes the database asynchronously.
        /// </summary>
        private async Task InitializeDatabaseAsync()
        {
            try
            {
                await _customerRepository.InitializeDatabaseAsync();
                Console.WriteLine("Database initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database: {ex.Message}");
                throw new Exception($"Error initializing database: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lists all customers in the database asynchronously.
        /// </summary>
        private async Task GetAllCustomersAsync()
        {
            Console.WriteLine("Listing all customers...");
            var customers = await _customerRepository.GetAllCustomersAsync();
            foreach (var customer in customers)
            {
                Console.WriteLine($"ID: {customer.Id}, Name: {customer.FirstName} {customer.LastName}, Email: {customer.Email}");
                Console.WriteLine($"Phone: {customer.Phone}, Country: {customer.Country}, Postal Code: {customer.PostalCode}");
                Console.WriteLine(new string('-', 50));
            }
        }

        private async Task GetCustomerByNameAsync()
        {
            Console.Write("Enter customer name to search: ");
            string searchQuery = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                Console.WriteLine("Search query cannot be empty.");
                return;
            }

            try
            {
                var customer = await _customerRepository.GetCustomerByNameAsync(searchQuery);

                if (customer != null)
                {
                    Console.WriteLine($"Customer found:");
                    Console.WriteLine($"ID: {customer.Id}");
                    Console.WriteLine($"Name: {customer.FirstName} {customer.LastName}");
                    Console.WriteLine($"Email: {customer.Email}");
                    Console.WriteLine($"Phone: {customer.Phone}");
                    Console.WriteLine($"Country: {customer.Country}");
                    Console.WriteLine($"Postal Code: {customer.PostalCode}");
                }
                else
                {
                    Console.WriteLine("No customer found with the given name.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching for customer: {ex.Message}");
            }
        }
        private async Task GetCustomerByIdAsync()
        {
            Console.Write("Enter customer ID to search: ");
            if (!int.TryParse(Console.ReadLine()?.Trim(), out int customerId))
            {
                Console.WriteLine("Invalid ID format. Please enter a valid numeric ID.");
                return;
            }

            try
            {
                var customer = await _customerRepository.GetCustomerByIdAsync(customerId);

                if (customer != null)
                {
                    Console.WriteLine($"Customer found:");
                    Console.WriteLine($"ID: {customer.Id}");
                    Console.WriteLine($"Name: {customer.FirstName} {customer.LastName}");
                    Console.WriteLine($"Email: {customer.Email}");
                    Console.WriteLine($"Phone: {customer.Phone}");
                    Console.WriteLine($"Country: {customer.Country}");
                    Console.WriteLine($"Postal Code: {customer.PostalCode}");
                }
                else
                {
                    Console.WriteLine("No customer found with the given ID.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching for customer: {ex.Message}");
            }
        }
    private async Task GetCustomersPagedAsync()
    {
        Console.Write("Enter the number of customers to retrieve (limit): ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out int limit) || limit <= 0)
        {
            Console.WriteLine("Invalid limit. Please enter a valid positive number.");
            return;
        }

        Console.Write("Enter the offset (number of customers to skip): ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out int offset) || offset < 0)
        {
            Console.WriteLine("Invalid offset. Please enter a valid non-negative number.");
            return;
        }

        try
        {
            var customers = await _customerRepository.GetCustomersPagedAsync(limit, offset);

            if (customers != null && customers.Any())
            {
                Console.WriteLine($"Retrieved {customers.Count()} customers:");
                foreach (var customer in customers)
                {
                    Console.WriteLine($"ID: {customer.Id}");
                    Console.WriteLine($"Name: {customer.FirstName} {customer.LastName}");
                    Console.WriteLine($"Email: {customer.Email}");
                    Console.WriteLine($"Phone: {customer.Phone}");
                    Console.WriteLine($"Country: {customer.Country}");
                    Console.WriteLine($"Postal Code: {customer.PostalCode}");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("No customers found for the given limit and offset.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving customers: {ex.Message}");
            }
        }
        
    private async Task GetCustomerCountByCountryDescAsync()
    {
        try
        {
            var customerCounts = await _customerRepository.GetCustomerCountByCountryDescAsync();

            if (customerCounts != null && customerCounts.Any())
            {
                Console.WriteLine("Customer count by country (descending):");
                foreach (var customerCountry in customerCounts)
                {
                    Console.WriteLine($"Country: {customerCountry.Name}, Customer Count: {customerCountry.CustomerCount}");
                }
            }
            else
            {
                Console.WriteLine("No customer count data found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving customer count by country: {ex.Message}");
        }
    }
    private async Task GetHighSpendersDescendingAsync()
    {
        try
        {
            var highSpenders = await _customerRepository.GetHighSpendersDescendingAsync();

            if (highSpenders != null && highSpenders.Any())
            {
                Console.WriteLine("High spenders (descending):");
                foreach (var spender in highSpenders)
                {
                    Console.WriteLine($"ID: {spender.Id}");
                    Console.WriteLine($"Name: {spender.FirstName} {spender.LastName}");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("No high spenders data found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving high spenders: {ex.Message}");
        }
    }
    private async Task GetMostPopularGenresByCustomerIdAsync()
    {
        try
        {
            Console.Write("Enter customer ID: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("Invalid customer ID. Please enter a valid number.");
                return;
            }

            var popularGenres = await _customerRepository.GetMostPopularGenresByCustomerIdAsync(customerId);

            if (popularGenres != null && popularGenres.Any())
            {
                Console.WriteLine($"Most popular genres for customer ID {customerId}:");
                foreach (var genre in popularGenres)
                {
                    Console.WriteLine($"Genre: {genre.Name}, Customer Count: {genre.CustomerCount}, Purchase Count: {genre.PurchaseCount}, Track Count: {genre.TrackCount}");
                }
            }
            else
            {
                Console.WriteLine("No popular genres data found for the specified customer.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving popular genres: {ex.Message}");
        }
    }

    }
}
    