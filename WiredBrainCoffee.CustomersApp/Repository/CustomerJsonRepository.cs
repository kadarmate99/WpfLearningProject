using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.Json;
using WiredBrainCoffee.CustomersApp.Configuration;
using WiredBrainCoffee.CustomersApp.Model;

namespace WiredBrainCoffee.CustomersApp.Repository
{
    public class CustomerJsonRepository : IRepository<Customer>
    {
        private readonly string _customersFilePath;
        private readonly JsonSerializerOptions _jsonOptions;


        // TODO – Currently, all methods are void; it would be useful to return something, e.g., the number of elements.
        // TODO – Implement unit tests for this class.
        // TODO – Implement SQLite DB with EF: https://github.com/praeclarum/sqlite-net


        public CustomerJsonRepository(IOptions<RepoConfig> options, JsonSerializerOptions jsonOptions)
        {
            var opts = options.Value;
            _customersFilePath = opts.CustomersFilePath;
            _jsonOptions = jsonOptions;
        }

        public IEnumerable<Customer> GetAll()
        {
            if (!File.Exists(_customersFilePath))
                return Enumerable.Empty<Customer>();

            string fileContent = File.ReadAllText(_customersFilePath);

            if (string.IsNullOrWhiteSpace(fileContent))
                return Enumerable.Empty<Customer>();

            var customers = JsonSerializer.Deserialize<IEnumerable<Customer>>(fileContent);
            return customers;
        }

        public void SaveAll(IEnumerable<Customer> customers)
        {
            string json = JsonSerializer.Serialize(customers, _jsonOptions);
            File.WriteAllText(_customersFilePath, json);
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
            // Replaced original delete method that used ToList() + FirstOrDefault() + Remove(), which:
            // 1) Loaded the entire collection into memory
            // 2) Searched it twice
            // 3) Modified it in-place
            // This version avoids ToList(), uses deferred execution, and creates filtered enumerable that only
            // materializes once in SaveAll() - much more memory efficient. Extra critical for databases because
            // ToList() would execute "SELECT * FROM Customers" pulling ALL records across the network into app memory,
            // while this keeps operations at DB level using optimized EXISTS and filtered SELECT queries.
            var customers = GetAll();
            if (!customers.Any(x => x.Id == customer.Id))
            {
                throw new InvalidOperationException();
            }

            SaveAll(customers.Where(c => c.Id != customer.Id));
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
