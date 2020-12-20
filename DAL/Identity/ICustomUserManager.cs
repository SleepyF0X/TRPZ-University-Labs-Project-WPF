using System;
using System.Threading.Tasks;
using DAL.DbModels.Identity.IdentityModels;

namespace DALServices.Identity
{
    public interface ICustomUserManager
    {
        Task<User> FindByEmail(string userName);
        Task<User> FindById(Guid userId);
        Task<bool> CreateUser(User user);
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(User user);
        Task<bool> UserExists(string userName);
    }
}