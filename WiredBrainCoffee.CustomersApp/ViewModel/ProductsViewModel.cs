using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WiredBrainCoffee.CustomersApp.Model;
using WiredBrainCoffee.CustomersApp.Repository;

namespace WiredBrainCoffee.CustomersApp.ViewModel
{
    public class ProductsViewModel : ViewModelBase
    {
        private readonly IRepository<Product> _productDataProvider;

        public ProductsViewModel(IRepository<Product> productDataProvider)
        {
            _productDataProvider = productDataProvider;
        }

        // A ViewModell for the product could be created,
        // but because this will be just a read only screen we can use the Product class as well
        public ObservableCollection<Product> Products { get; } = new();

        public override void Load()
        {
            if (Products.Any())
            {
                return;
            }

            var products = _productDataProvider.GetAll();
            if (products is not null)
            {
                foreach (var product in products)
                {
                    Products.Add(product);
                }
            }
        }
    }
}
