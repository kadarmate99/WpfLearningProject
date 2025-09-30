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
    public class ProductJsonRepository : IRepository<Product>
    {
        private readonly string _productsFilePath;
        private readonly JsonSerializerOptions _jsonOptions;


        public ProductJsonRepository(IOptions<RepoConfig> options, JsonSerializerOptions jsonOptions)
        {
            var opts = options.Value;
            _productsFilePath = opts.ProductsFilePath;
            _jsonOptions = jsonOptions;
        }
        public IEnumerable<Product> GetAll()
        {
            if (!File.Exists(_productsFilePath))
                return Enumerable.Empty<Product>();

            string fileContent = File.ReadAllText(_productsFilePath);
            var products = JsonSerializer.Deserialize<IEnumerable<Product>>(fileContent) ?? Enumerable.Empty<Product>();
            return products;
        }

        public void SaveAll(IEnumerable<Product> products)
        {
            string json = JsonSerializer.Serialize(products, _jsonOptions);
            File.WriteAllText(_productsFilePath, json);
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