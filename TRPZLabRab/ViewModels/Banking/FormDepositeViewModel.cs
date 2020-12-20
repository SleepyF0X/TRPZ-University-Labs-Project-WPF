using System;
using DAL.DbModels;
using DevExpress.Mvvm;
using TRPZLabRab.Additionals;

namespace TRPZLabRab.ViewModels.Banking
{
    public sealed class FormDepositeViewModel : ViewModelBase
    {
        private readonly decimal _userBalance;
        public FormDepositeViewModel()
        {
        }

        public FormDepositeViewModel(Deposite depositeToEdit)
        {
            Id = depositeToEdit.Id;
            StartSum = depositeToEdit.StartSum;
            Years = depositeToEdit.Years;
        }

        public FormDepositeViewModel(decimal userBalance)
        {
            _userBalance = userBalance;
        }

        public Guid Id
        {
            get => GetValue<Guid>();
            set => SetValue(value);
        }


        public decimal StartSum
        {
            get => GetValue<decimal>();
            set => SetValue(value);
        }

        public int Years
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public string ErrorMessage
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public bool IsValid()
        {
            if (StartSum > _userBalance)
            {
                ErrorMessage = Errors.MaxValue(nameof(StartSum), Convert.ToInt32(_userBalance));
                return false;
            }

            if (StartSum < 0)
            {
                ErrorMessage = Errors.MinValue(nameof(StartSum), 0);
                return false;
            }
            if (Years < 1)
            {
                ErrorMessage = Errors.MinValue(nameof(Years), 1);
                return false;
            }

            if (Years > 24)
            {
                ErrorMessage = Errors.MaxValue(nameof(Years), 24);
                return false;
            }
            ErrorMessage = string.Empty;
            return true;
        }
    }
}
