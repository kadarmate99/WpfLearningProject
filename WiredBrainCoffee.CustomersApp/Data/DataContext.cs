using Microsoft.EntityFrameworkCore;
using WiredBrainCoffee.CustomersApp.Model;

namespace WiredBrainCoffee.CustomersApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Espresso", Description = "Strong black coffee" },
                new Product { Id = 2, Name = "Cappuccino", Description = "Espresso with steamed milk and foam" },
                new Product { Id = 3, Name = "Latte", Description = "Espresso with lots of steamed milk" },
                new Product { Id = 4, Name = "Americano", Description = "Espresso with hot water" },
                new Product { Id = 5, Name = "Mocha", Description = "Chocolate flavored coffee drink" },
                new Product { Id = 6, Name = "Cold Brew", Description = "Coffee steeped in cold water" },
                new Product { Id = 7, Name = "Macchiato", Description = "Espresso marked with foam" }
            );

            modelBuilder.Entity<Customer>().HasData(
                new Customer { Id = 1, FirstName = "Julia", LastName = "Developer", IsDeveloper = true },
                new Customer { Id = 2, FirstName = "Anna", LastName = "Tester", IsDeveloper = false },
                new Customer { Id = 3, FirstName = "Thomas", LastName = "Manager", IsDeveloper = false }
            );
        }
    }
}
