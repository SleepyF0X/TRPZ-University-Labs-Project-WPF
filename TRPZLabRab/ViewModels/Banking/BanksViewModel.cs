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
    public sealed class BanksViewModel : ViewModelBase
    {
        private readonly IAuthenticator _authenticator;
        private readonly IBankRepository _bankRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICustomUserManager _userManager;
        public ObservableCollection<Bank> Banks { get; set; }
        public ObservableCollection<Bank> UserBanks { get; set; }
        public string AdminButtonsVisibility
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string UserBanksVisibility
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public BanksViewModel(IAuthenticator authenticator, IBankRepository bankRepository, IAccountRepository accountRepository, ICustomUserManager userManager)
        {
            _authenticator = authenticator;
            _bankRepository = bankRepository;
            _accountRepository = accountRepository;
            _userManager = userManager;
            Banks = new ObservableCollection<Bank>();
            UserBanks = new ObservableCollection<Bank>();
            Messenger.Default.Register<RefreshMessage>(this, RefreshDataAsync);
            CreateBankCommand = new RelayCommand(CreateBankHandler);
            AddClientCommand = new RelayCommand(bankId=>AddClientHandler((Guid)bankId));
            GetClientInfo = new RelayCommand(bankId => Messenger.Default.Send(new RoutingMessage(nameof(ClientInfoViewModel), (Guid)bankId)));
            AdminButtonsVisibility = "Collapsed";
        }
        private void CurrentUserIsClientAnyBank()
        {
            var user = _authenticator.GetCurrentUser();
            if (user != null)
            {
                var isAnyBank = _authenticator.GetCurrentUser().Accounts.Select(a => a.Bank).Any();
                UserBanksVisibility = isAnyBank ? "Visible" : "Collapsed";
                UserBanks = _authenticator.GetCurrentUser().Accounts.Select(a => a.Bank).ToObservableCollection();
            }
            else UserBanksVisibility = "Collapsed";
        }
        private async Task CurrentUserIsAdmin()
        {
            var isAdmin = await _authenticator.CurrentUserIsInRole("Admin");
            AdminButtonsVisibility = isAdmin ? "Visible" : "Collapsed";
        }

        public async void RefreshDataAsync(RefreshMessage refreshDataMessage)
        {
            if (GetType().Name.Equals(refreshDataMessage.ViewModelName))
            {
                Banks.Clear();
                var banks = (await _bankRepository.GetAll()).Where(b=>!_authenticator.GetCurrentUser().Accounts.Select(a => a.Bank).Select(ub=>ub.Id).Contains(b.Id));
                foreach (var bank in banks)
                {
                    Banks.Add(bank);
                }
                CurrentUserIsClientAnyBank();
                await CurrentUserIsAdmin();

            }
        }

        public ICommand CreateBankCommand { get; set; }
        public ICommand AddClientCommand { get; set; }
        public ICommand GetClientInfo { get; set; }
        private async void CreateBankHandler()
        {
            var view = new FormBankDialog()
            {
                DataContext = new FormBankViewModel()
            };

            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
            if (result is FormBankViewModel data)
            {
                var createdBank = new Bank(data.Name, data.Balance);
                await _bankRepository.AddOne(createdBank);
                Banks.Add(createdBank);
            }
        }
        private async void AddClientHandler(Guid bankId)
        {
            var view = new FormClientDialog()
            {
                DataContext = new FormClientViewModel(_authenticator)
            };

            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
            if (result is FormClientViewModel data)
            {
                var createdClient = new Account(_authenticator.GetCurrentUser().Id, data.Balance, 1000,bankId);
                await _accountRepository.AddOne(createdClient);
                var user = _authenticator.GetCurrentUser();
                user.Balance -= data.Balance;
                await _userManager.UpdateUser(user);
                await _authenticator.UpdateUserData();
                var bank = Banks.FirstOrDefault(b=>b.Id.Equals(bankId));
                Banks.Remove(bank);
                bank.Balance += data.Balance;
                await _bankRepository.UpdateOne(bank);
                UserBanks.Add(bank);
                Messenger.Default.Send(new RefreshMessage(nameof(MainViewModel)));
            }
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            var dialogHost = (DialogHost) sender;
            var dialogSession = dialogHost.CurrentSession;
            if (dialogSession.Content.GetType() == typeof(FormBankDialog) &&
                eventArgs.Parameter is FormBankViewModel dialogViewModel)
            {
                if (!dialogViewModel.IsValid())
                {
                    eventArgs.Cancel();
                }
                else if (Banks.Any(e =>
                    e.Name.ToLower().Equals(dialogViewModel.Name.ToLower()) &&
                    !e.Id.Equals(dialogViewModel.Id)))
                {
                    dialogViewModel.ErrorMessage =
                        $"Bank with same name already exists on {dialogViewModel.Name}.";
                    eventArgs.Cancel();
                }
            }
            else if(dialogSession.Content.GetType() == typeof(FormClientDialog) &&
                    eventArgs.Parameter is FormClientViewModel dialogViewModel2)
            {
                if (!dialogViewModel2.IsValid())
                {
                    eventArgs.Cancel();
                }
            }
        }
    }
}
