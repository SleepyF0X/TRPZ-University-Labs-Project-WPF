using System;
using System.Threading.Tasks;
using DAL;
using DAL.DbModels.Identity.IdentityModels;
using DALServices.Identity;
using Microsoft.EntityFrameworkCore;

namespace DALServicesImpl.Identity
{
    public sealed class CustomUserManager : ICustomUserManager
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public CustomUserManager(DbContextOptions<AppDbContext> options)
        {
            _options = options;
        }

        public async Task<User> FindByEmail(string userName)
        {
            await using var context = new AppDbContext(_options);
            if (!string.IsNullOrEmpty(userName))
            {
                var user = await context.Users.Include(u=>u.Accounts).ThenInclude(a=>a.Bank).FirstOrDefaultAsync(
                    e => e.Email.ToLower().Equals(userName.ToLower()));
                return user;
            }

            return null;
        }

        public async Task<User> FindById(Guid userId)
        {
            await using var context = new AppDbContext(_options);
            if (!userId.Equals(Guid.Empty))
            {
                var user = await context.Users.Include(u=>u.Accounts).ThenInclude(a=>a.Bank).FirstOrDefaultAsync(e => e.Id.Equals(userId));
                return user;
            }

            return null;
        }

        public async Task<bool> CreateUser(User user)
        {
            await using var context = new AppDbContext(_options);
            if (user != null)
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateUser(User user)
        {
            await using var context = new AppDbContext(_options);
            if (user != null)
            {
                context.Users.Update(user);
                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteUser(User user)
        {
            await using var context = new AppDbContext(_options);
            if (user != null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> UserExists(string userName)
        {
            await using var context = new AppDbContext(_options);
            if (!string.IsNullOrEmpty(userName))
                return await context.Users.AnyAsync(e => e.Email.ToLower().Equals(userName.ToLower()));
            return false;
        }
    }
}