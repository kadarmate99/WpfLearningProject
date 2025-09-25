using System.Reflection.Metadata;
using WiredBrainCoffee.CustomersApp.Command;
using WiredBrainCoffee.CustomersApp.Model;
using WiredBrainCoffee.CustomersApp.Repository;

namespace WiredBrainCoffee.CustomersApp.ViewModel
{
    public class CustomerItemViewModel : ValidationViewModelBase
    {
        private readonly Customer _originalModel;
        private readonly ICustomerRepository _repository;
        private readonly Customer _editedModel;

        public CustomerItemViewModel(Customer model, ICustomerRepository repository)
        {
            _originalModel = model;
            _repository = repository;
            _editedModel = new Customer
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                IsDeveloper = model.IsDeveloper
            };

            ApplyChangesCommand = new DelegateCommand(ApplyChanges, CanApplyChanges);
            DiscardChangesCommand = new DelegateCommand(DiscardChanges);

        }
        /// <summary> 
        /// Gets the data object backing this view model.
        /// </summary>
        public Customer OriginalCustomer => _originalModel;

        public DelegateCommand ApplyChangesCommand { get; }
        public DelegateCommand DiscardChangesCommand { get; }

        #region Display Properties for ListView, only valid data should be set with apply chnages method
        public string? FirstName
        {
            get => OriginalCustomer.FirstName;
            private set 
            {
                OriginalCustomer.FirstName = value;
                RaisePropertyChanged();

            }
        } 
        public string? LastName
        {
            get => OriginalCustomer.LastName;
            private set
            {
                OriginalCustomer.LastName = value;
                RaisePropertyChanged();

            }
        }
        public bool IsDeveloper 
        { 
            get => OriginalCustomer.IsDeveloper;
            private set
            {
                OriginalCustomer.IsDeveloper = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Properties we let directly edit - displayed in textbox / checkbox
        public string? EditedFirstName
        {
            get => _editedModel.FirstName;
            set
            {
                _editedModel.FirstName = value;

                ClearErrors();
                if (string.IsNullOrEmpty(_editedModel.FirstName))
                    AddError("First name is required");

                RaisePropertyChanged();
                ApplyChangesCommand.RaiseCanExecuteChanged();
            }
        }
        public string? EditedLastName
        {
            get => _editedModel.LastName;
            set
            {
                _editedModel.LastName = value;

                ClearErrors();
                if (string.IsNullOrEmpty(_editedModel.LastName))
                    AddError("First name is required");

                RaisePropertyChanged();
                ApplyChangesCommand.RaiseCanExecuteChanged();
            }
        }
        public bool EditedIsDeveloper
        {
            get => _editedModel.IsDeveloper;
            set
            {
                _editedModel.IsDeveloper = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private bool CanApplyChanges(object? parameter) =>!HasErrors;

        private void ApplyChanges(object? parameter)
        {
            if (!CanApplyChanges(parameter)) return;

            FirstName = EditedFirstName;
            LastName = EditedLastName;
            IsDeveloper = EditedIsDeveloper;

            _repository.Update(OriginalCustomer);
        }

        private void DiscardChanges(object? parameter)
        {
            EditedFirstName = FirstName;
            EditedLastName = LastName;
            EditedIsDeveloper = IsDeveloper;
        }
    }
}
