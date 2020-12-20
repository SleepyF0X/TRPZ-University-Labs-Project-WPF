using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DbModels;
using DevExpress.Mvvm;
using TRPZLabRab.Additionals;

namespace TRPZLabRab.ViewModels.Banking
{
    public sealed class FormCreditViewModel : ViewModelBase
    {
        private readonly Bank _bank;

        public FormCreditViewModel()
        {
        }

        public FormCreditViewModel(Credit creditToEdit)
        {
            Id = creditToEdit.Id;
            StartSum = creditToEdit.StartSum;
            Months = creditToEdit.Months;
        }

        public FormCreditViewModel(Bank bank)
        {
            _bank = bank;
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

        public int Months
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
            if (StartSum > _bank.Balance)
            {
                ErrorMessage = Errors.MaxValue(nameof(StartSum), Convert.ToInt32(_bank.Balance));
                return false;
            }

            if (StartSum < 0)
            {
                ErrorMessage = Errors.MinValue(nameof(StartSum), 0);
                return false;
            }
            if (Months < 1)
            {
                ErrorMessage = Errors.MinValue(nameof(Months), 1);
                return false;
            }

            if (Months > 24)
            {
                ErrorMessage = Errors.MaxValue(nameof(Months), 24);
                return false;
            }
            ErrorMessage = string.Empty;
            return true;
        }
    }
}
