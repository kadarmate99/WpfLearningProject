using System.Collections.Generic;
using WiredBrainCoffee.CustomersApp.Model;

namespace WiredBrainCoffee.CustomersApp.Repository
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAll();
        void SaveAll(IEnumerable<Product> products);
    }
}