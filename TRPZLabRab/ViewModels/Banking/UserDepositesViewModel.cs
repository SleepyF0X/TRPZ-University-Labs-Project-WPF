using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DAL.DbModels;
using DAL.IRepositories;
using DALServices.Identity;
using DALServices.IRepositories;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using MaterialDesignThemes.Wpf;
using TRPZLabRab.AdminArea.Controls;
using TRPZLabRab.AdminArea.ViewModels;
using TRPZLabRab.Messages;

namespace TRPZLabRab.ViewModels.Banking
{
    public sealed class UserDepositesViewModel : ViewModelBase
    {
        private readonly IAuthenticator _authenticator;

        private readonly IDepositeRepository _depositeRepository;

        public ObservableCollection<Deposite> Deposites { get; set; }

        public UserDepositesViewModel(IAuthenticator authenticator,
            IDepositeRepository depositeRepository)
        {
            _authenticator = authenticator;
            _depositeRepository = depositeRepository;
            Deposites = new ObservableCollection<Deposite>();
            Messenger.Default.Register<RefreshMessage>(this, RefreshDataAsync);
            NavigateToClientInfo = new RelayCommand(bankId => HandleNavigation(new RoutingMessage(nameof(ClientInfoViewModel), (Guid)bankId)));
        }

        public async void RefreshDataAsync(RefreshMessage refreshDataMessage)
        {
            if (GetType().Name.Equals(refreshDataMessage.ViewModelName))
            {
                Deposites.Clear();
                var deposites = (await _depositeRepository.GetAll()).Where(d =>
                    d.Client.User.Id.Equals(_authenticator.GetCurrentUser().Id)).OrderBy(d=>d.StartDate).Reverse();
                foreach (var deposite in deposites)
                {
                    Deposites.Add(deposite);
                }
            }
        }

        public ICommand NavigateToClientInfo { get; set; }

        private void HandleNavigation(RoutingMessage navigationMessage)
        {
            Messenger.Default.Send(new RoutingMessage(navigationMessage.ViewModelName, navigationMessage.ItemId));
        }
    }
}
