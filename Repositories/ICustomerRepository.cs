using ChinookApp.Models;

namespace ChinookSuperheroes.Repositories
{
    public interface ICustomerRepository
    {
        Task InitializeDatabaseAsync();
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task<Customer?> GetCustomerByNameAsync(string query);
        Task AddCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(int id);
        Task<IEnumerable<Customer>> GetCustomersPagedAsync(int limit, int offset);
        Task<IEnumerable<CustomerCountry>> GetCustomerCountByCountryDescAsync();
        Task<IEnumerable<Customer>> GetHighSpendersDescendingAsync();
        Task<IEnumerable<CustomerGenre>> GetMostPopularGenresByCustomerIdAsync(int customerId);
    }
}
