using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WiredBrainCoffee.CustomersApp.Model;

namespace WiredBrainCoffee.CustomersApp.Repository
{
    public class ProductJsonRepository : IProductRepository
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
    }
}