using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using WiredBrainCoffee.CustomersApp.Model;

namespace WiredBrainCoffee.CustomersApp.Repository
{
    public class ProductJsonRepository : IRepository<Product>
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "products.json");
        JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };

        public IEnumerable<Product> GetAll()
        {
            string fileContent = File.ReadAllText(filePath);
            var products = JsonSerializer.Deserialize<IEnumerable<Product>>(fileContent) ?? Enumerable.Empty<Product>();
            return products;
        }

        public void SaveAll(IEnumerable<Product> products)
        {
            string json = JsonSerializer.Serialize(products, options);
            File.WriteAllText(filePath, json);
        }

        public void Add(Product product)
        {
            var products = GetAll().ToList();
            products.Add(product);
            SaveAll(products);
        }

        public void Delete(Product product)
        {
            var products = GetAll().ToList();
            var existingCustomer = products.FirstOrDefault(p => p.Name == product.Name);

            if (existingCustomer is null)
                throw new InvalidOperationException();

            products.Remove(existingCustomer);

            SaveAll(products);
        }


        public void Update(Product product)
        {
            var products = GetAll().ToList();
            var existingCustomer = products.FirstOrDefault(p => p.Name == product.Name);

            if (existingCustomer is null)
                throw new InvalidOperationException();

            existingCustomer.Description = product.Description;

            SaveAll(products);
        }
    }
}