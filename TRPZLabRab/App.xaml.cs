using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ChainStore.DataAccessLayerImpl.Helpers;
using DAL;
using DAL.DbModels.Identity.IdentityModels;
using DAL.IRepositories;
using DALImpl;
using DALImpl.Repositories;
using DALServices.Identity;
using DALServices.IRepositories;
using DALServicesImpl.Identity;
using DALServicesImpl.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TRPZLabRab.ViewModels;
using TRPZLabRab.ViewModels.Banking;
using TRPZLabRab.ViewModels.Identity;

namespace TRPZLabRab
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = CreateHostBuilder().Build();
        }

        public static IConfiguration Config { get; private set; }

        public static IHostBuilder CreateHostBuilder(string[] args = null)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(c =>
                {
                    c.AddJsonFile("appsettings.json");
                    c.AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    Config = context.Configuration;
                    services.AddDbContext<AppDbContext>(opt =>
                    {
                        opt.UseSqlServer(context.Configuration.GetConnectionString("TRPZDB"));
                    });
                    services.AddSingleton<OptionsBuilderService<AppDbContext>>();
                    services.AddSingleton<ITransactionRepository, EFTransactionRepository>();
                    services.AddSingleton<IAccountRepository, EFAccountRepository>();
                    services.AddSingleton<ICreditRepository, EFCreditRepository>();
                    services.AddSingleton<IDepositeRepository, EFDepositeRepository>();
                    services.AddSingleton<IBankRepository, EFBankRepository>();
                    services.AddSingleton<ICustomUserManager, CustomUserManager>();
                    services.AddSingleton<ICustomRoleManager, CustomRoleManager>();
                    services.AddSingleton<IAuthenticationService, AuthenticationService>();
                    services.AddSingleton<IAuthenticator, Authenticator>();
                    services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
                    services.AddSingleton<RegisterViewModel>();
                    services.AddSingleton<LoginViewModel>();
                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<BanksViewModel>();
                    services.AddSingleton<ClientInfoViewModel>();
                    services.AddSingleton<UserDepositesViewModel>();
                    services.AddSingleton<UserCreditsViewModel>();
                });
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();
            var window = _host.Services.GetRequiredService<MainWindow>();
            window.DataContext = _host.Services.GetRequiredService<MainViewModel>();
            window.Show();
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }
    }
}
