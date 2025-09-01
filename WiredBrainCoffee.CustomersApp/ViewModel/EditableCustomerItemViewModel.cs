using WiredBrainCoffee.CustomersApp.Model;

namespace WiredBrainCoffee.CustomersApp.ViewModel
{
    public class EditableCustomerItemViewModel : ValidationViewModelBase
    {
        private string? _firstName;
        private string? _lastName;
        private bool _isDeveloper;

        public string? FirstName
        {
            get => _firstName;
            set
            { 
                _firstName = value;
                RaisePropertyChanged();

                if(string.IsNullOrEmpty(_firstName))
                {
                    AddError("First name is required");
                }
                else
                {
                    ClearErrors();
                }
            }
        }

        public string? LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                RaisePropertyChanged();

                if (string.IsNullOrEmpty(_lastName))
                {
                    AddError("Last name is required");
                }
                else
                {
                    ClearErrors();
                }
            }
        }

        public bool IsDeveloper
        {
            get => _isDeveloper;
            set
            {
                _isDeveloper = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// Loads data FROM the specified <see cref="CustomerItemViewModel"/>.
        /// </summary>
        public void LoadFrom(CustomerItemViewModel customer)
        {
            FirstName = customer.FirstName;
            LastName = customer.LastName;
            IsDeveloper = customer.IsDeveloper;
        }

        /// <summary>
        /// Loads data TO the specified <see cref="CustomerItemViewModel"/>.
        /// </summary>
        public void SaveTo(CustomerItemViewModel customer)
        {
            customer.FirstName = FirstName;
            customer.LastName = LastName;
            customer.IsDeveloper = IsDeveloper;
        }

        public bool IsValid()
        {
            return !HasErrors;
        }
    }
}
