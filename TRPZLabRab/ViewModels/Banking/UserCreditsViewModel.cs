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
using TRPZLabRab.Messages;

namespace TRPZLabRab.ViewModels.Banking
{
    public sealed class UserCreditsViewModel:ViewModelBase
    {
        private readonly IAuthenticator _authenticator;

        private readonly ICreditRepository _CreditRepository;

        public ObservableCollection<Credit> Credits { get; set; }

        public UserCreditsViewModel(IAuthenticator authenticator,
            ICreditRepository CreditRepository)
        {
            _authenticator = authenticator;
            _CreditRepository = CreditRepository;
            Credits = new ObservableCollection<Credit>();
            Messenger.Default.Register<RefreshMessage>(this, RefreshDataAsync);
            NavigateToClientInfo = new RelayCommand(bankId => HandleNavigation(new RoutingMessage(nameof(ClientInfoViewModel), (Guid)bankId)));
        }

        public async void RefreshDataAsync(RefreshMessage refreshDataMessage)
        {
            if (GetType().Name.Equals(refreshDataMessage.ViewModelName))
            {
                Credits.Clear();
                var credits = (await _CreditRepository.GetAll()).Where(d =>
                    d.Client.User.Id.Equals(_authenticator.GetCurrentUser().Id)).OrderBy(d=>d.StartDate).Reverse();
                foreach (var credit in credits)
                {
                    Credits.Add(credit);
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
