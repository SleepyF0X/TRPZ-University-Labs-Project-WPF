using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using DAL.DbModels;
using DALServices.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DALServicesImpl.Repositories
{
    public sealed class EFTransactionRepository : ITransactionRepository
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public EFTransactionRepository(DbContextOptions<AppDbContext> options)
        {
            _options = options;
        }

        public async Task AddOne(Transaction item)
        {
            await using var context = new AppDbContext(_options);
            if (!Exists(item.Id))
            {
                var enState = await context.Transactions.AddAsync(item);
                enState.State = EntityState.Added;
                await context.SaveChangesAsync();
            }
        }

        public async Task<Transaction> GetOne(Guid id)
        {
            await using var context = new AppDbContext(_options);
            if (Exists(id))
            {
                var transaction = await context.Transactions.FindAsync(id);
                return transaction;
            }
            return null;
        }

        public async Task<IReadOnlyCollection<Transaction>> GetAll()
        {
            await using var context = new AppDbContext(_options);
            return context.Transactions.ToList().AsReadOnly();
        }

        public async Task UpdateOne(Transaction item)
        {
            await using var context = new AppDbContext(_options);
            if (Exists(item.Id))
            {
                    var enState = context.Transactions.Update(item);
                    enState.State = EntityState.Modified;
                    await context.SaveChangesAsync();
            }
        }

        public async Task DeleteOne(Guid id)
        {
            await using var context = new AppDbContext(_options);
            if (Exists(id))
            {
                var transaction = await context.Transactions.FindAsync(id);
                var enState = context.Transactions.Remove(transaction);
                enState.State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
        }

        public bool Exists(Guid id)
        {
            using var context = new AppDbContext(_options);
            return context.Transactions.Any(item => item.Id.Equals(id));
        }
    }
}
