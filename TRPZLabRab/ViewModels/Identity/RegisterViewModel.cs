﻿using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using DALServices.Identity;
using DevExpress.Mvvm;
using TRPZLabRab.Messages;

namespace TRPZLabRab.ViewModels.Identity
{
    public sealed class RegisterViewModel : ViewModelBase
    {
        #region Constructor

        public RegisterViewModel(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
            Register = new RelayCommand(async passwordInputBoxes => await HandleRegistration(passwordInputBoxes));
            NavigateToLogin = new RelayCommand(() =>
            {
                ClearData();
                Messenger.Default.Send(new RoutingMessage(nameof(LoginViewModel)));
            });
        }

        #endregion

        #region Handlers

        private async Task HandleRegistration(object passwordInputBoxes)
        {
            var unpackedPasswordInputBoxes = (object[]) passwordInputBoxes;
            var validationSucceeded = Validate(unpackedPasswordInputBoxes);
            if (validationSucceeded)
            {
                var tryRegister = await _authenticator.Register(Name, Surname, Age, Email,
                    ((PasswordBox) unpackedPasswordInputBoxes[0]).Password,
                    ((PasswordBox) unpackedPasswordInputBoxes[1]).Password);
                if (tryRegister == RegistrationResult.Success)
                    NavigateToLogin.Execute(null);
                else if (tryRegister == RegistrationResult.EmailAlreadyTaken)
                    ErrorMessage = "Email is already taken.";
                else
                    ErrorMessage = "Something went wrong. Try to register later.";
            }
        }

        #endregion

        #region Properties

        private readonly IAuthenticator _authenticator;

        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string Surname
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public int Age
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public string Email
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string ErrorMessage
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        #endregion

        #region Commands

        public ICommand NavigateToLogin { get; set; }
        public ICommand Register { get; set; }

        #endregion

        #region Methods

        public void ClearData()
        {
            Name = string.Empty;
            Email = string.Empty;
            ErrorMessage = string.Empty;
        }


        private bool Validate(object[] passwords)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");
            if (string.IsNullOrEmpty(Name) || Name.Length < 2 || Name.Length > 60)
            {
                ErrorMessage = "Provide valid name.";
                return false;
            }

            try
            {
                var email = new MailAddress(Email);
            }
            catch (Exception)
            {
                ErrorMessage = "Provide valid email.";
                return false;
            }

            if (passwords == null || !hasMinimum8Chars.IsMatch(((PasswordBox) passwords[0]).Password))
            {
                ErrorMessage = "Password must contain at least 6 characters.";
                return false;
            }

            if (!hasUpperChar.IsMatch(((PasswordBox) passwords[0]).Password))
            {
                ErrorMessage = "Password must contain at least 1 upper.";
                return false;
            }

            if (!hasNumber.IsMatch(((PasswordBox) passwords[0]).Password))
            {
                ErrorMessage = "Password must contain at least 1 number.";
                return false;
            }

            if (((PasswordBox) passwords[0]).Password != ((PasswordBox) passwords[1]).Password)
            {
                ErrorMessage = "Passwords do not match.";
                return false;
            }

            return true;
        }

        #endregion
    }
}