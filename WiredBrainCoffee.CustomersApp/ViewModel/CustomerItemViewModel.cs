using System.Reflection.Metadata;
using WiredBrainCoffee.CustomersApp.Command;
using WiredBrainCoffee.CustomersApp.Model;

namespace WiredBrainCoffee.CustomersApp.ViewModel
{
    public class CustomerItemViewModel : ValidationViewModelBase
    {
        private readonly Customer _originalModel;
        private readonly Customer _editedModel;

        public CustomerItemViewModel(Customer model)
        {
            _originalModel = model;
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

        public DelegateCommand ApplyChangesCommand { get; }
        public DelegateCommand DiscardChangesCommand { get; }

        #region Display Properties for ListView, only valid data should be set with apply chnages method
        public string? FirstName
        {
            get => _originalModel.FirstName;
            private set 
            {
                _originalModel.FirstName = value;
                RaisePropertyChanged();

            }
        } 
        public string? LastName
        {
            get => _originalModel.LastName;
            private set
            {
                _originalModel.LastName = value;
                RaisePropertyChanged();

            }
        }
        public bool IsDeveloper 
        { 
            get => _originalModel.IsDeveloper;
            private set
            {
                _originalModel.IsDeveloper = value;
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
        }

        private void DiscardChanges(object? parameter)
        {
            EditedFirstName = FirstName;
            EditedLastName = LastName;
            EditedIsDeveloper = IsDeveloper;
        }
    }
}
