using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using DAL.DbModels;
using DALServices.Identity;
using DevExpress.Mvvm;
using MaterialDesignThemes.Wpf;
using TRPZLabRab.AdminArea.Controls;
using TRPZLabRab.AdminArea.ViewModels;
using TRPZLabRab.Controls;
using TRPZLabRab.Messages;
using TRPZLabRab.ViewModels.Banking;
using TRPZLabRab.ViewModels.Identity;

namespace TRPZLabRab.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase GetAppropriateViewModel(string viewModelName)
        {
            return ViewModels.Find(e => e.GetType().Name.Equals(viewModelName));
        }


        #region Properties

        private readonly IAuthenticator _authenticator;
        private readonly ICustomUserManager _userManager;
        public List<ViewModelBase> ViewModels { get; }

        private ViewModelBase _currentViewModel;
        private string _menuIconVisibility;
        private int _menuIconZIndex;
        private string _username;
        private decimal _balance;
        public ICommand ChangeBalance { get; set; }

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                SetValue(value);
            }
        }

        public string MenuIconVisibility
        {
            get => _menuIconVisibility;
            set
            {
                _menuIconVisibility = value;
                SetValue(value);
            }
        }

        public int MenuIconZIndex
        {
            get => _menuIconZIndex;
            set
            {
                _menuIconZIndex = value;
                SetValue(value);
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                SetValue(value);
            }
        }
        public decimal Balance
        {
            get => _balance;
            set
            {
                _balance = value;
                SetValue(value);
            }
        }
        #endregion

        #region Navigational Commands

        public ICommand NavigateToLogin { get; set; }
        public ICommand NavigateToBanks { get; set; }
        public ICommand NavigateToDeposites { get; set; }
        public ICommand NavigateToCredits { get; set; }
        public ICommand Logout { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        ///     Constructor for creating commands, handling messages
        /// </summary>
        public MainViewModel(IAuthenticator authenticator, ICustomUserManager userManager)
        {
            _authenticator = authenticator;
            _userManager = userManager;
            NavigateToLogin = new RelayCommand(() =>
            {
                CurrentViewModel = GetAppropriateViewModel(nameof(LoginViewModel));
            });
            NavigateToBanks = new RelayCommand(() =>
            {
                if (_authenticator.IsLoggedIn())
                    HandleNavigation(new RoutingMessage(nameof(BanksViewModel)));
                else
                    MessageBox.Show("Login to access stores.");
            });
            NavigateToDeposites = new RelayCommand(() =>
            {
                if (_authenticator.IsLoggedIn())
                    HandleNavigation(new RoutingMessage(nameof(UserDepositesViewModel)));
                else
                    MessageBox.Show("Login to access stores.");
            });
            NavigateToCredits = new RelayCommand(() =>
            {
                if (_authenticator.IsLoggedIn())
                    HandleNavigation(new RoutingMessage(nameof(UserCreditsViewModel)));
                else
                    MessageBox.Show("Login to access stores.");
            });
            Logout = new RelayCommand(() =>
            {
                _authenticator.Logout();
                HandleMenuIcon(new LoginMessage(false));
                NavigateToLogin.Execute(null);
            });
            ChangeBalance = new RelayCommand(() =>
            {
                if (_authenticator.IsLoggedIn())
                    ReplenishBalance();
                else
                    MessageBox.Show("Login to access stores.");
            });
            Messenger.Default.Register<RoutingMessage>(this, HandleNavigation);
            Messenger.Default.Register<LoginMessage>(this, HandleMenuIcon);
            Messenger.Default.Register<RefreshMessage>(this, HandleBalance);
        }

        private void HandleBalance(RefreshMessage obj)
        {
            Balance = _authenticator.GetCurrentUser() != null ? _authenticator.GetCurrentUser().Balance : 0;
        }

        /// <summary>
        ///     Constructor for injecting ViewModels
        /// </summary>
        /// <param name="authenticator">For providing current user's email in sidebar menu</param>
        /// <param name="registerViewModel"></param>
        /// <param name="loginViewModel"></param>
        /// <param name="storeViewModel"></param>
        /// <param name="storeDetailsViewModel"></param>
        /// <param name="purchaseViewModel"></param>
        /// <param name="bookViewModel"></param>
        /// <param name="profileViewModel"></param>
        /// <param name="categoriesViewModel"></param>
        public MainViewModel(IAuthenticator authenticator, RegisterViewModel registerViewModel,
            LoginViewModel loginViewModel, BanksViewModel banksViewModel, ClientInfoViewModel clientInfoViewModel, UserDepositesViewModel userDepositesViewModel, UserCreditsViewModel userCreditsViewModel, ICustomUserManager userManager) : this(authenticator, userManager)
        {
            Username = "Unauthorized";
            MenuIconVisibility = "Collapsed";
            ViewModels = new List<ViewModelBase>
            {
                registerViewModel, loginViewModel, banksViewModel, clientInfoViewModel, userDepositesViewModel, userCreditsViewModel
            };
            CurrentViewModel = GetAppropriateViewModel(nameof(LoginViewModel));
        }

        #endregion

        #region Handlers

        private void HandleNavigation(RoutingMessage navigationMessage)
        {
            Messenger.Default.Send(new RefreshMessage(navigationMessage.ViewModelName, navigationMessage.ItemId));
            CurrentViewModel = GetAppropriateViewModel(navigationMessage.ViewModelName);
        }

        private void HandleMenuIcon(LoginMessage loginMessage)
        {
            if (loginMessage.IsLoggedIn)
            {
                Username = _authenticator.GetCurrentUser().Email;
                MenuIconVisibility = "Visible";
                MenuIconZIndex = 2;
                Balance = _authenticator.GetCurrentUser().Balance;
            }
            else
            {
                Username = "Unauthorized";
                MenuIconVisibility = "Collapsed";
                MenuIconZIndex = 0;
            }
        }
        private async void ReplenishBalance()
        {
            var view = new FormBalanceDialog()
            {
                DataContext = new FormBalanceViewModel()
            };

            var result = await DialogHost.Show(view, "RootDialog");
            if (result is FormBalanceViewModel data)
            {
                var userId = _authenticator.GetCurrentUser().Id;
                var user = await _userManager.FindById(userId);
                user.Balance += data.Sum;
                await _userManager.UpdateUser(user);
                await _authenticator.UpdateUserData();
                Balance += data.Sum;
            }
        }
        #endregion
    }
}
