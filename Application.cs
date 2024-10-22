using System;
using System.Data.SqlClient;
using ChinookSuperheroes.Repositories;
using System.Threading.Tasks;

namespace ChinookSuperheroes
{
    public class Application
    {
        private readonly string _connectionString;
        private readonly ICustomerRepository _customerRepository;

        public Application(string connectionString, ICustomerRepository customerRepository)
        {
            _connectionString = connectionString;
            _customerRepository = customerRepository;
        }

        public async Task RunAsync(string[] args)
        {
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
                            AddSuperhero();
                            break;
                        case "power":
                            AddPower();
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
                            ListSuperheroes();
                            break;
                        case "powers":
                            ListPowers();
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

        private async Task InitializeDatabaseAsync()
        {
            Console.WriteLine("Initializing database...");
            // Implement database initialization logic here
        }

        private void AddSuperhero()
        {
            Console.WriteLine("Adding a new superhero...");
            // Implement superhero addition logic here
        }

        private void AddPower()
        {
            Console.WriteLine("Adding a new superpower...");
            // Implement superpower addition logic here
        }

        private void ListSuperheroes()
        {
            Console.WriteLine("Listing all superheroes...");
            // Implement superhero listing logic here
        }

        private void ListPowers()
        {
            Console.WriteLine("Listing all superpowers...");
            // Implement superpower listing logic here
        }

        private async Task ListCustomersAsync()
        {
            Console.WriteLine("Listing all customers...");
            var customers = await _customerRepository.GetAllCustomersAsync();
            foreach (var customer in customers)
            {
                Console.WriteLine($"ID: {customer.Id}, Name: {customer.Name}");
            }
        }
    }
}
