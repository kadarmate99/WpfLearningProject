using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WiredBrainCoffee.CustomersApp.Data;
using WiredBrainCoffee.CustomersApp.Model;

namespace WiredBrainCoffee.CustomersApp.Repository
{
    public class CustomerEfRepository : IRepository<Customer>
    {
        private readonly DataContext _context;

        public CustomerEfRepository(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<Customer> Add(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
            return GetAll();
        }

        public IEnumerable<Customer> GetAll()
        {
            return _context.Customers.ToList();
        }

        public IEnumerable<Customer> Update(Customer customer)
        {
            var existingCustomer = _context.Customers.Find(customer.Id);
            if (existingCustomer is null)
                throw new InvalidOperationException($"The specified customer ({customer.FirstName} {customer.LastName}, ID {customer.Id}) does not exist.");

            _context.Entry(existingCustomer).CurrentValues.SetValues(customer);

            _context.SaveChanges();
            return GetAll();
        }

        public IEnumerable<Customer> Delete(Customer customer)
        {
            _context.Customers.Remove(customer);
            _context.SaveChanges();
            return GetAll();
        }
    }
}
