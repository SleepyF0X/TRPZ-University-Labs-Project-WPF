using System.Threading.Tasks;
using DAL.DbModels.Identity.IdentityModels;
using DALServices.Identity;

namespace DALServicesImpl.Identity
{
    public sealed class Authenticator : IAuthenticator
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ICustomRoleManager _customRoleManager;
        private readonly ICustomUserManager _customUserManager;
        public Authenticator(IAuthenticationService authenticationService, ICustomRoleManager customRoleManager, ICustomUserManager customUserManager)
        {
            _authenticationService = authenticationService;
            _customRoleManager = customRoleManager;
            _customUserManager = customUserManager;
        }

        public User CurrentUser { get; private set; }

        public bool IsLoggedIn()
        {
            return CurrentUser != null;
        }

        public async Task<bool> Login(string email, string password)
        {
            var user = await _authenticationService.Login(email, password);
            CurrentUser = user;
            return CurrentUser != null;
        }

        public async Task<RegistrationResult> Register(string name, string surname, int age, string email, string password, string confirmPassword)
        {
            return await _authenticationService.Register(name, surname, age, email, password, confirmPassword);
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        public User GetCurrentUser()
        {
            return CurrentUser;
        }

        public async Task<bool> CurrentUserIsInRole(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName) && IsLoggedIn())
                return await _customRoleManager.IsInRole(CurrentUser, roleName);
            return false;
        }

        public async Task UpdateUserData()
        {
            CurrentUser = await _customUserManager.FindById(CurrentUser.Id);
        }
    }
}