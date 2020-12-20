using System.Threading.Tasks;
using DAL.DbModels.Identity.IdentityModels;

namespace DALServices.Identity
{
    public interface IAuthenticator
    {
        bool IsLoggedIn();
        Task<bool> Login(string email, string password);
        Task<RegistrationResult> Register(string name, string surname, int age, string email, string password, string confirmPassword);
        void Logout();
        User GetCurrentUser();
        Task<bool> CurrentUserIsInRole(string roleName);
        Task UpdateUserData();
    }
}