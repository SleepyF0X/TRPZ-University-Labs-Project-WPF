using System;
using DAL.DbModels;
using DevExpress.Mvvm;
using TRPZLabRab.Additionals;

namespace TRPZLabRab.AdminArea.ViewModels
{
    public sealed class FormBankViewModel : ViewModelBase
    {
        public FormBankViewModel()
        {
        }

        public FormBankViewModel(Bank bankToEdit)
        {
            Id = bankToEdit.Id;
            Name = bankToEdit.Name;
            Balance = bankToEdit.Balance;
        }

        public Guid Id
        {
            get => GetValue<Guid>();
            set => SetValue(value);
        }

        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public decimal Balance
        {
            get => GetValue<decimal>();
            set => SetValue(value);
        }

        public string ErrorMessage
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Name))
            {
                ErrorMessage = Errors.Required(nameof(Name));
                return false;
            }

            if (Name.Length < 2)
            {
                ErrorMessage = Errors.StringMinLength(nameof(Name), 2);
                return false;
            }

            if (Name.Length > 60)
            {
                ErrorMessage = Errors.StringMaxLength(nameof(Name), 60);
                return false;
            }

            if (Balance>1000000000)
            {
                ErrorMessage = Errors.MaxValue(nameof(Balance), 1000000000);
                return false;
            }

            if (Balance<0)
            {
                ErrorMessage = Errors.MinValue(nameof(Balance), 0);
                return false;
            }

            ErrorMessage = string.Empty;
            return true;
        }
    }
}