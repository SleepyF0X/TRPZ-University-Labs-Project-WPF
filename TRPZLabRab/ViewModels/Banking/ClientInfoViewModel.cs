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
using TRPZLabRab.Controls;
using TRPZLabRab.Messages;

namespace TRPZLabRab.ViewModels.Banking
{
    public sealed class ClientInfoViewModel : ViewModelBase
    {
        private readonly IAuthenticator _authenticator;
        private readonly IBankRepository _bankRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IDepositeRepository _depositeRepository;
        private readonly ICreditRepository _creditRepository;
        private readonly ITransactionRepository _transactionRepository;
        public ObservableCollection<Credit> Credits { get; set; }
        public ObservableCollection<Deposite> Deposites { get; set; }
        public ObservableCollection<Transaction> Transactions { get; set; }
        public ICommand CreateCreditCommand { get; set; }
        public ICommand CreateDepositeCommand { get; set; }
        public ICommand GetClientInfo { get; set; }
        public string AdminButtonsVisibility
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public Guid BankId
        {
            get => GetValue<Guid>();
            set => SetValue(value);
        }
        public decimal AccountBalance
        {
            get => GetValue<decimal>();
            set => SetValue(value);
        }
        public decimal BankBalance
        {
            get => GetValue<decimal>();
            set => SetValue(value);
        }
        public ClientInfoViewModel(IAuthenticator authenticator, IBankRepository bankRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository, ICreditRepository creditRepository, IDepositeRepository depositeRepository)
        {
            _authenticator = authenticator;
            _bankRepository = bankRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _creditRepository = creditRepository;
            _depositeRepository = depositeRepository;
            Credits = new ObservableCollection<Credit>();
            Deposites = new ObservableCollection<Deposite>();
            Transactions = new ObservableCollection<Transaction>();
            Messenger.Default.Register<RefreshMessage>(this, RefreshDataAsync);
            CreateCreditCommand = new RelayCommand(CreateCreditHandler);
            CreateDepositeCommand = new RelayCommand(CreateDepositeHandler);
            GetClientInfo = new RelayCommand(bankId => Messenger.Default.Send(new RoutingMessage(nameof(ClientInfoViewModel))));
            AdminButtonsVisibility = "Collapsed";
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
                BankId = refreshDataMessage.ItemId.Value;
                AccountBalance =_authenticator.GetCurrentUser().Accounts.FirstOrDefault(a => a.Bank.Id.Equals(BankId)).Balance;
                BankBalance = (await _bankRepository.GetOne(BankId)).Balance;
                ClearData();
                var credits =(await _creditRepository.GetAll())
                    .Where(c=>c.ClientId.Equals(_authenticator.GetCurrentUser().Accounts.FirstOrDefault(a=>a.BankId.Equals(BankId)).Id)&&c.BankId.Equals(BankId))
                    .OrderBy(t=>t.StartDate).Reverse();
                var deposites =(await _depositeRepository.GetAll())
                    .Where(c=>c.ClientId.Equals(_authenticator.GetCurrentUser().Accounts.FirstOrDefault(a=>a.BankId.Equals(BankId)).Id)&&c.BankId.Equals(BankId))
                    .OrderBy(t=>t.StartDate).Reverse();
                var transactions = (await _transactionRepository.GetAll())
                    .Where(c => c.ClientId.Equals(_authenticator.GetCurrentUser().Accounts.FirstOrDefault(a=>a.BankId.Equals(BankId)).Id) && c.BankId.Equals(BankId))
                    .OrderBy(t=>t.Date).Reverse();
                foreach (var transaction in transactions)
                {
                    Transactions.Add(transaction);
                }
                foreach (var deposite in deposites)
                {
                    Deposites.Add(deposite);
                }

                foreach (var credit in credits)
                {
                    Credits.Add(credit);
                }
                await CurrentUserIsAdmin();
            }
        }

        public void ClearData()
        {
            Credits.Clear();
            Deposites.Clear();
            Transactions.Clear();
        }

        private async void CreateCreditHandler()
        {
            var view = new FormCreditDialog()
            {
                DataContext = new FormCreditViewModel(await _bankRepository.GetOne(BankId))
            };

            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
            if (result is FormCreditViewModel data)
            {
                var PercentagePerMonth = 0;
                if (data.Months < 3)
                {
                    PercentagePerMonth = 5;
                }
                else if(data.Months>=4&&data.Months<=7)
                {
                    PercentagePerMonth = 9;
                }
                else
                {
                    PercentagePerMonth = 15;
                }

                var account = _authenticator.GetCurrentUser().Accounts.FirstOrDefault(a => a.Bank.Id.Equals(BankId));
                var accountDb = await _accountRepository.GetOne(account.Id);
                accountDb.Balance += data.StartSum;
                await _accountRepository.UpdateOne(accountDb);
                var bank = await _bankRepository.GetOne(BankId);
                bank.Balance -= data.StartSum;
                await _bankRepository.UpdateOne(bank);
                var createdCredit = new Credit(_authenticator.GetCurrentUser().Accounts.FirstOrDefault(a=>a.BankId.Equals(BankId)).Id,BankId, DateTimeOffset.Now, data.StartSum, PercentagePerMonth, data.Months);
                await _creditRepository.AddOne(createdCredit);
                Credits.Add(createdCredit);
                Credits.Move(Credits.Count-1, 0);
                await _authenticator.UpdateUserData();
                AccountBalance += data.StartSum;
                BankBalance -= data.StartSum;
                var transaction = (await _transactionRepository.GetAll()).OrderBy(t=>t.Date).Last(t => t.ClientId.Equals(account.Id)&&t.BankId.Equals(bank.Id));
                Transactions.Add(transaction);
                Transactions.Move(Transactions.Count-1, 0);
                Messenger.Default.Send(new RefreshMessage(nameof(MainViewModel)));
            }
        }
        private async void CreateDepositeHandler()
        {
            var view = new FormDepositeDialog()
            {
                DataContext = new FormDepositeViewModel(_authenticator.GetCurrentUser().Balance)
            };

            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
            if (result is FormDepositeViewModel data)
            {
                var PercentagePerMonth = 0;
                if (data.Years < 3)
                {
                    PercentagePerMonth = 5;
                }
                else if(data.Years>=4&&data.Years<=7)
                {
                    PercentagePerMonth = 9;
                }
                else
                {
                    PercentagePerMonth = 15;
                }
                var account = _authenticator.GetCurrentUser().Accounts.FirstOrDefault(a => a.Bank.Id.Equals(BankId));
                var accountDb = await _accountRepository.GetOne(account.Id);
                accountDb.Balance -= data.StartSum;
                await _accountRepository.UpdateOne(accountDb);
                var bank = await _bankRepository.GetOne(BankId);
                bank.Balance += data.StartSum;
                await _bankRepository.UpdateOne(bank);
                var createdDeposite = new Deposite(_authenticator.GetCurrentUser().Accounts.FirstOrDefault(a=>a.BankId.Equals(BankId)).Id,BankId, DateTimeOffset.Now, data.StartSum, PercentagePerMonth, data.Years);
                await _depositeRepository.AddOne(createdDeposite);
                Deposites.Add(createdDeposite);
                Deposites.Move(Deposites.Count-1, 0);
                await _authenticator.UpdateUserData();
                AccountBalance -= data.StartSum;
                BankBalance += data.StartSum;
                var transaction = (await _transactionRepository.GetAll()).OrderBy(t=>t.Date).Last(t => t.ClientId.Equals(account.Id)&&t.BankId.Equals(bank.Id));
                Transactions.Add(transaction);
                Transactions.Move(Transactions.Count-1, 0);
                Messenger.Default.Send(new RefreshMessage(nameof(MainViewModel)));
            }
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            var dialogHost = (DialogHost)sender;
            var dialogSession = dialogHost.CurrentSession;
            if (dialogSession.Content.GetType() == typeof(FormDepositeDialog) &&
                eventArgs.Parameter is FormDepositeViewModel dialogViewModel)
            {
                if (!dialogViewModel.IsValid())
                {
                    eventArgs.Cancel();
                }
            }
            else if(dialogSession.Content.GetType() == typeof(FormCreditDialog) &&
                eventArgs.Parameter is FormCreditViewModel dialogViewModel2)
            {
                if (!dialogViewModel2.IsValid())
                {
                    eventArgs.Cancel();
                }
            }
        }
    }
}
