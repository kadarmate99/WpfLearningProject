using System.Collections.Generic;
using WiredBrainCoffee.CustomersApp.Model;

namespace WiredBrainCoffee.CustomersApp.Repository
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetAll();
        void SaveAll(IEnumerable<Customer> customers);

        void Add(Customer customer);
        void Update(Customer customer);
        void Delete(Customer customer);
    }
}
