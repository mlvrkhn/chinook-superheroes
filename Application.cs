using ChinookApp.Models;
using ChinookSuperheroes.Repositories;

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
                    if (args.Length < 2) 
                    {
                        Console.WriteLine("Please specify what to add: superhero or power");
                        return;
                    }
                    switch (args[1].ToLower())
                    {
                        case "superhero":
                            await AddSuperheroAsync();
                            break;
                        case "power":
                            await AddPowerAsync();
                            break;
                        default:
                            Console.WriteLine("Unknown add command. Use 'superhero' or 'power'.");
                            break;
                    }
                    break;
                case "list":
                    if (args.Length < 2) 
                    {
                        Console.WriteLine("Please specify what to list: superheroes or powers");
                        return;
                    }
                    switch (args[1].ToLower())
                    {
                        case "superheroes":
                            await ListSuperheroesAsync();
                            break;
                        case "powers":
                            await ListPowersAsync();
                            break;
                        case "customers":
                            await ListCustomersAsync();
                            break;
                        default:
                            Console.WriteLine("Unknown list command. Use 'superheroes', 'powers', or 'customers'.");
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("Unknown command. Use 'init', 'add', or 'list'.");
                    ShowHelp();
                    break;
            }
        }

        private void ShowHelp()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("  init - Initialize the database");
            Console.WriteLine("  add superhero - Add a new superhero");
            Console.WriteLine("  add power - Add a new superpower");
            Console.WriteLine("  list superheroes - List all superheroes");
            Console.WriteLine("  list powers - List all superpowers");
            Console.WriteLine("  list customers - List all customers");
        }
        private async Task AddCustomerAsync()
        {
            Console.WriteLine("Adding a new customer...");
            Console.Write("Enter customer's first name: ");
            string firstName = Console.ReadLine() ?? string.Empty;
            Console.Write("Enter customer's last name: ");
            string lastName = Console.ReadLine() ?? string.Empty;

            var newCustomer = new Customer
            {
                FirstName = firstName,
                LastName = lastName
            };

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

        private async Task UpdateCustomerAsync()
        {
            Console.WriteLine("Updating a customer...");
            Console.Write("Enter customer ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("Invalid customer ID.");
                return;
            }

            var existingCustomer = await _customerRepository.GetCustomerByIdAsync(customerId);
            if (existingCustomer == null)
            {
                Console.WriteLine("Customer not found.");
                return;
            }

            Console.Write($"Enter new first name (current: {existingCustomer.FirstName}): ");
            string firstName = Console.ReadLine() ?? existingCustomer.FirstName;
            Console.Write($"Enter new last name (current: {existingCustomer.LastName}): ");
            string lastName = Console.ReadLine() ?? existingCustomer.LastName;

            existingCustomer.FirstName = firstName;
            existingCustomer.LastName = lastName;

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
                throw; // Re-throw the exception to stop execution if initialization fails
            }
        }

        private async Task AddSuperheroAsync()
        {
            Console.WriteLine("Adding a new superhero...");
            await AddCustomerAsync();
        }

        private async Task AddPowerAsync()
        {
            Console.WriteLine("Adding a new superpower...");
            await AddCustomerAsync();
        }

        private async Task ListSuperheroesAsync()
        {
            Console.WriteLine("Listing all superheroes...");
            await ListCustomersAsync();
        }

        private async Task ListPowersAsync()
        {
            Console.WriteLine("Listing all superpowers...");
            await AddCustomerAsync();
        }

        private async Task ListCustomersAsync()
        {
            Console.WriteLine("Listing all customers...");
            var customers = await _customerRepository.GetAllCustomersAsync();
            foreach (var customer in customers)
            {
                Console.WriteLine($"ID: {customer.Id}, Name: {customer.FirstName}, Lastname: {customer.LastName}");
            }
        }
    }
}
