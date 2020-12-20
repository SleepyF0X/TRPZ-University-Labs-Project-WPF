using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DbModels;
using DALServices.Identity;
using DevExpress.Mvvm;
using TRPZLabRab.Additionals;

namespace TRPZLabRab.ViewModels.Banking
{
    public sealed class FormClientViewModel : ViewModelBase
    {
        private readonly IAuthenticator _authenticator;
        public FormClientViewModel(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
        }

        public FormClientViewModel(Account clientToEdit, IAuthenticator authenticator)
        {
            Id = clientToEdit.Id;
            Balance = clientToEdit.Balance;
            _authenticator = authenticator;
        }

        public Guid Id
        {
            get => GetValue<Guid>();
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

            if (_authenticator.GetCurrentUser().Balance <= Balance)
            {
                ErrorMessage = "Balance low";
                return false;
            }
            ErrorMessage = string.Empty;
            return true;
        }
    }
}
