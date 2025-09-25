using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WiredBrainCoffee.CustomersApp.Model;

namespace WiredBrainCoffee.CustomersApp.Repository
{
    public class CustomerJsonRepository : IRepository<Customer>
    {
        private readonly string filePath = Path.Combine(AppContext.BaseDirectory, "people.json");
        private readonly JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };

        public IEnumerable<Customer> GetAll()
        {
            string fileContent = File.ReadAllText(filePath);
            var customers = JsonSerializer.Deserialize<IEnumerable<Customer>>(fileContent) ?? Enumerable.Empty<Customer>();
            return customers;
        }

        public void SaveAll(IEnumerable<Customer> customers)
        {
            string json = JsonSerializer.Serialize(customers, options);
            File.WriteAllText(filePath, json);
        }

        public void Add(Customer customer)
        {
            var customers = GetAll().ToList();

            var newId = customers.Any() ? (customers.Max(c => c.Id) + 1) : 1;
            customer.Id = newId;
            customers.Add(customer);

            SaveAll(customers);
        }

        public void Delete(Customer customer)
        {
            var customers = GetAll().ToList();
            var existingCustomer = customers.FirstOrDefault(c => c.Id == customer.Id);

            if (existingCustomer is null)
                throw new InvalidOperationException();

            customers.Remove(existingCustomer);

            SaveAll(customers);
        }

        public void Update(Customer customer)
        {
            var customers = GetAll().ToList();
            var existingCustomer = customers.FirstOrDefault(c => c.Id == customer.Id);

            if (existingCustomer is null)
                throw new InvalidOperationException();

            existingCustomer.FirstName = customer.FirstName;
            existingCustomer.LastName = customer.LastName;
            existingCustomer.IsDeveloper = customer.IsDeveloper;

            SaveAll(customers);
        }
    }
}
