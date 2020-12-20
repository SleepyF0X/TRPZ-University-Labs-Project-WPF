using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using DAL.DbModels;
using DAL.DbModels.Enums;
using DALServices.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DALServicesImpl.Repositories
{
    public sealed class EFDepositeRepository : IDepositeRepository
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public EFDepositeRepository(DbContextOptions<AppDbContext> options)
        {
            _options = options;
        }

        public async Task AddOne(Deposite item)
        {
            await using var context = new AppDbContext(_options);
            if (!Exists(item.Id))
            {
                var enState = await context.Deposites.AddAsync(item);
                enState.State = EntityState.Added;
                await context.Transactions.AddAsync(new Transaction(DateTimeOffset.Now, TransactionType.DepositeOpen,
                    item.StartSum, item.ClientId, item.BankId, item.Id));
                (await context.Banks.FindAsync(item.BankId)).Balance += item.StartSum;
                await context.SaveChangesAsync();
            }
        }

        public async Task<Deposite> GetOne(Guid id)
        {
            await using var context = new AppDbContext(_options);
            if (Exists(id))
            {
                var deposite = await context.Deposites.FindAsync(id);
                return deposite;
            }
            return null;
        }

        public async Task<IReadOnlyCollection<Deposite>> GetAll()
        {
            await using var context = new AppDbContext(_options);
            return context.Deposites.Include(d=>d.Client).ThenInclude(c=>c.User).Include(d=>d.Bank).ToList().AsReadOnly();
        }

        public async Task UpdateOne(Deposite item)
        {
            await using var context = new AppDbContext(_options);
            if (Exists(item.Id))
            {
                    var enState = context.Deposites.Update(item);
                    enState.State = EntityState.Modified;
                    await context.SaveChangesAsync();
            }
        }

        public async Task DeleteOne(Guid id)
        {
            await using var context = new AppDbContext(_options);
            if (Exists(id))
            {
                var deposite = await context.Deposites.FindAsync(id);
                var enState = context.Deposites.Remove(deposite);
                enState.State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
        }

        public bool Exists(Guid id)
        {
            using var context = new AppDbContext(_options);
            return context.Deposites.Any(item => item.Id.Equals(id));
        }
    }
}
