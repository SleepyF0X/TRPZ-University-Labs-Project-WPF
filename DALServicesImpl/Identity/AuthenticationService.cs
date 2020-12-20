using System;
using System.Threading.Tasks;
using DAL.DbModels;
using DAL.DbModels.Identity.IdentityModels;
using DALServices.Identity;
using DALServices.IRepositories;
using Microsoft.AspNetCore.Identity;

namespace DALServicesImpl.Identity
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly IAccountRepository _clientRepository;
        private readonly ICustomRoleManager _customRoleManager;
        private readonly ICustomUserManager _customUserManager;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthenticationService(ICustomUserManager customUserManager, ICustomRoleManager customRoleManager,
            IPasswordHasher<User> passwordHasher, IAccountRepository clientRepository)
        {
            _customUserManager = customUserManager;
            _customRoleManager = customRoleManager;
            _passwordHasher = passwordHasher;
            _clientRepository = clientRepository;
        }

        public async Task<User> Login(string email, string password)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var user = await _customUserManager.FindByEmail(email);
                if (user == null) return null;
                var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.HashPassword, password);
                return verificationResult == PasswordVerificationResult.Success ? user : null;
            }

            return null;
        }

        public async Task<RegistrationResult> Register(string name, string surname, int age, string email, string password, string confirmPassword)
        {
            if (password != confirmPassword) return RegistrationResult.Fail;
                var userWithProvidedEmailExists = await _customUserManager.UserExists(email);
                if (userWithProvidedEmailExists) return RegistrationResult.EmailAlreadyTaken;
                var user = new User(email, name, surname, age);
                var hashedPassword = _passwordHasher.HashPassword(user, password);
                user.SetPassword(hashedPassword);
                await _customUserManager.CreateUser(user);
                var roleExists = await _customRoleManager.RoleExists("Client");
                if (!roleExists) await _customRoleManager.CreateRole(new Role("Client"));
                await _customRoleManager.AddToRole(user, "Client");
                return RegistrationResult.Success;
        }
    }
}