using System.Threading.Tasks;
using DAL.DbModels.Identity.IdentityModels;

namespace DALServices.Identity
{
    public interface IAuthenticationService
    {
        Task<User> Login(string email, string password);
        Task<RegistrationResult> Register(string name, string surname, int age, string email, string password, string confirmPassword);
    }
}