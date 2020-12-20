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
    public class FormBalanceViewModel : ViewModelBase
    {

        public FormBalanceViewModel()
        {
        }

        public decimal Sum
        {
            get => GetValue<decimal>();
            set => SetValue(value);
        }

    }
}
