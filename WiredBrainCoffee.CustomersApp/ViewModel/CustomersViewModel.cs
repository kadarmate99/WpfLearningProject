using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using WiredBrainCoffee.CustomersApp.Command;
using WiredBrainCoffee.CustomersApp.Data;
using WiredBrainCoffee.CustomersApp.Model;

namespace WiredBrainCoffee.CustomersApp.ViewModel
{
    public class CustomersViewModel : ViewModelBase
    {
        private readonly ICustomerDataProvider _customerDataProvider;
        private CustomerItemViewModel? _selectedCustomer;
        private NavigationSide _navigationSide;
        private EditableCustomerItemViewModel? _currentCustomerEdit;

        public CustomersViewModel(ICustomerDataProvider customerDataProvider)
        {
            _customerDataProvider = customerDataProvider;
            AddCommand = new DelegateCommand(Add);
            MoveNavigationCommand = new DelegateCommand(MoveNavigation);
            DeleteCommand = new DelegateCommand(Delete, CanDelete);
            SaveCommand = new DelegateCommand(Save, CanSave);
            RevertCommand = new DelegateCommand(Revert);
        }

        public ObservableCollection<CustomerItemViewModel> Customers { get; } = new();

        public EditableCustomerItemViewModel? CurrentCustomerEdit
        {
            get => _currentCustomerEdit;
            set
            {
                // Unsubscribe from previous instance
                if (_currentCustomerEdit != null)
                {
                    _currentCustomerEdit.PropertyChanged -= OnCurrentCustomerEditPropertyChanged;
                }

                _currentCustomerEdit = value;

                // Subscribe to new instance  
                if (_currentCustomerEdit != null)
                {
                    _currentCustomerEdit.PropertyChanged += OnCurrentCustomerEditPropertyChanged;
                }

                _currentCustomerEdit = value;
                RaisePropertyChanged();
            }
        }


        public CustomerItemViewModel? SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                // TODO: find a better solution for updating the Save button availability thane this manual event subscription. 
                _selectedCustomer = value;

                if (_selectedCustomer != null)
                {
                    CurrentCustomerEdit = new EditableCustomerItemViewModel();
                    CurrentCustomerEdit.LoadFrom(_selectedCustomer);
                }
                else
                {
                    CurrentCustomerEdit = null;
                }

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

        public DelegateCommand SaveCommand { get; }

        public DelegateCommand RevertCommand { get; }

        public async override Task LoadAsync()
        {
            if (Customers.Any())
            {
                return;
            }

            var customers = await _customerDataProvider.GetAllAsync();
            if (customers is not null)
            {
                foreach (var customer in customers)
                {
                    Customers.Add(new CustomerItemViewModel(customer));
                }
            }
        }

        private void Add(object? parameter)
        {
            var customer = new Customer { FirstName = "New" };
            var viewModel = new CustomerItemViewModel(customer);
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
                Customers.Remove(SelectedCustomer);
                SelectedCustomer = null;
            }
        }

        private bool CanDelete(object? parameter) => SelectedCustomer is not null;

        private void Save(object? parameter)
        {
            if (CurrentCustomerEdit is not null && SelectedCustomer is not null && CurrentCustomerEdit.IsValid())
            {
                CurrentCustomerEdit.SaveTo(SelectedCustomer);
            }
        }

        private bool CanSave(object? parameter) =>
            CurrentCustomerEdit is not null &&
            SelectedCustomer is not null &&
            CurrentCustomerEdit.IsValid();

        private void Revert(object? parameter)
        {
            if (SelectedCustomer != null)
            {
                CurrentCustomerEdit = new EditableCustomerItemViewModel();
                CurrentCustomerEdit.LoadFrom(SelectedCustomer);
            }
            else
            {
                CurrentCustomerEdit = null;
            }
            //RaisePropertyChanged(nameof(CurrentCustomerEdit));
        }

        private void OnCurrentCustomerEditPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveCommand.RaiseCanExecuteChanged();
        }
    }

    public enum NavigationSide
    {
        Left,
        Right
    }
}
