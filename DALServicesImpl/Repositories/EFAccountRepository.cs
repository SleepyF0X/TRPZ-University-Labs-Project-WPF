using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChainStore.DataAccessLayerImpl.Helpers;
using DAL;
using DAL.DbModels;
using DALServices.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DALServicesImpl.Repositories
{
    public class EFAccountRepository : IAccountRepository
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public EFAccountRepository(OptionsBuilderService<AppDbContext> optionsBuilder)
        {
            _options = optionsBuilder.BuildOptions();
        }

        public async Task AddOne(Account item)
        {
            await using var context = new AppDbContext(_options);
            if (!Exists(item.Id))
            {
                var enState = await context.Accounts.AddAsync(item);
                enState.State = EntityState.Added;
                await context.SaveChangesAsync();
            }
        }

        public async Task<Account> GetOne(Guid id)
        {
            await using var context = new AppDbContext(_options);
            if (Exists(id))
            {
                var client = context.Accounts.Include(a => a.Bank).FirstOrDefault(a => a.Id.Equals(id));
                return client;
            }

            return null;
        }

        public async Task<IReadOnlyCollection<Account>> GetAll()
        {
            await using var context = new AppDbContext(_options);
            var clients = await context.Accounts.ToListAsync();
            return clients.AsReadOnly();
        }

        public async Task UpdateOne(Account item)
        {
            await using var context = new AppDbContext(_options);
            if (Exists(item.Id))
            {
                var enState = context.Accounts.Update(item);
                enState.State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteOne(Guid id)
        {
            await using var context = new AppDbContext(_options);
            if (Exists(id))
            {
                var client = await context.Accounts.FindAsync(id);
                var enState = context.Accounts.Remove(client);
                enState.State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
        }

        public bool Exists(Guid id)
        {
            using var context = new AppDbContext(_options);
            return context.Accounts.Any(item => item.Id.Equals(id));
        }
    }
}
