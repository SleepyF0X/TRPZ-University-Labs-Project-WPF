using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using DAL.DbModels;
using DAL.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DALImpl.Repositories
{
    public sealed class EFBankRepository : IBankRepository
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public EFBankRepository(DbContextOptions<AppDbContext> options)
        {
            _options = options;
        }

        public async Task AddOne(Bank item)
        {
            await using var context = new AppDbContext(_options);
            if (!Exists(item.Id))
            {
                var enState = await context.Banks.AddAsync(item);
                enState.State = EntityState.Added;
                await context.SaveChangesAsync();
            }
        }

        public async Task<Bank> GetOne(Guid id)
        {
            await using var context = new AppDbContext(_options);
            if (Exists(id))
            {
                var bank = await context.Banks.FindAsync(id);
                return bank;
            }

            return null;
        }

        public async Task<IReadOnlyCollection<Bank>> GetAll()
        {
            await using var context = new AppDbContext(_options);
            return context.Banks.ToList().AsReadOnly();
        }

        public async Task UpdateOne(Bank item)
        {
            await using var context = new AppDbContext(_options);
            if (Exists(item.Id))
            {
                var exists = await HasSameNameAsync(item);
                if (!exists)
                {
                    var enState = context.Banks.Update(item);
                    enState.State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteOne(Guid id)
        {
            await using var context = new AppDbContext(_options);
            if (Exists(id))
            {
                var bank = await context.Banks.FindAsync(id);
                var enState = context.Banks.Remove(bank);
                enState.State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
        }

        public bool Exists(Guid id)
        {
            using var context = new AppDbContext(_options);
            return context.Banks.Any(item => item.Id.Equals(id));
        }

        public async Task<bool> HasSameNameAsync(Bank bank)
        {
            await using var context = new AppDbContext(_options);
            if (bank != null)
                return await context.Banks.AnyAsync(b => b.Name.ToLower().Equals(bank.Name.ToLower()) && !b.Id.Equals(bank.Id));
            return false;
        }
    }
}
