using System;
using System.Collections.Generic;
using System.Linq;
using WiredBrainCoffee.CustomersApp.Data;
using WiredBrainCoffee.CustomersApp.Model;

namespace WiredBrainCoffee.CustomersApp.Repository
{
    public class ProductEfRepository : IRepository<Product>
    {
        private readonly DataContext _context;

        public ProductEfRepository(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<Product> Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return GetAll();
        }

        public IEnumerable<Product> GetAll()
        {
            return _context.Products.ToList();
        }

        public IEnumerable<Product> Update(Product product)
        {
            var existingProduct = _context.Products.Find(product.Id);
            if (existingProduct is null)
                throw new InvalidOperationException($"The specified product ({product.Name}, ID {product.Id}) does not exist.");

            _context.Entry(existingProduct).CurrentValues.SetValues(product);

            _context.SaveChanges();
            return GetAll();
        }

        public IEnumerable<Product> Delete(Product product)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
            return GetAll();
        }
    }
}
