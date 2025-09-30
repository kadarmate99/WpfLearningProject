using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using WiredBrainCoffee.CustomersApp.Command;
using WiredBrainCoffee.CustomersApp.Model;
using WiredBrainCoffee.CustomersApp.Repository;

namespace WiredBrainCoffee.CustomersApp.ViewModel
{
    public class CustomersViewModel : ValidationViewModelBase
    {
        private readonly IRepository<Customer> _repository;
        private CustomerItemViewModel? _selectedCustomer;
        private NavigationSide _navigationSide;
        private string? _customerFirstNameEdit;

        public CustomersViewModel(IRepository<Customer> repository)
        {
            _repository = repository;
            AddCommand = new DelegateCommand(Add);
            MoveNavigationCommand = new DelegateCommand(MoveNavigation);
            DeleteCommand = new DelegateCommand(Delete, CanDelete);
        }

        public ObservableCollection<CustomerItemViewModel> Customers { get; } = new();

        public CustomerItemViewModel? SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;

                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsCustomerSelected));
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsCustomerSelected => SelectedCustomer is not null;

        public NavigationSide NavigationSide
        {
            get => _navigationSide;
            private set
            {
                _navigationSide = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand AddCommand { get; }

        public DelegateCommand MoveNavigationCommand { get; }

        public DelegateCommand DeleteCommand { get; }

        public override void Load()
        {
            if (Customers.Any())
            {
                return;
            }

            var customers = _repository.GetAll();
            if (customers is not null)
            {
                foreach (var customer in customers)
                {
                    // DIRECT INSTANTIATION: Generally bad (violates DIP, hard to test, tight coupling)
                    // ACCEPTABLE HERE: Simple view model in same layer, natural parent-child relationship,
                    // no complex logic to mock.
                    Customers.Add(new CustomerItemViewModel(customer, _repository));
                }
            }

        }

        private void Add(object? parameter)
        {
            // Instantiation of Customer model is acceptable - it's just a data structure (DTO)
            var customer = new Customer
            {
                FirstName = "New",
                LastName = "New",
                IsDeveloper = false
            };

            // Same DI considerations as in Load() - acceptable for simple view model hierarchy
            var viewModel = new CustomerItemViewModel(customer, _repository);
            _repository.Add(customer);

            Customers.Add(viewModel);
            SelectedCustomer = viewModel;
        }

        private void MoveNavigation(object? parameter)
        {
            NavigationSide = NavigationSide == NavigationSide.Left
              ? NavigationSide.Right
              : NavigationSide.Left;
        }

        private void Delete(object? parameter)
        {
            if (SelectedCustomer is not null)
            {
                _repository.Delete(SelectedCustomer.OriginalCustomer);
                Customers.Remove(SelectedCustomer);

                SelectedCustomer = null;
            }
        }

        private bool CanDelete(object? parameter) => SelectedCustomer is not null;
    }

    public enum NavigationSide
    {
        Left,
        Right
    }
}
