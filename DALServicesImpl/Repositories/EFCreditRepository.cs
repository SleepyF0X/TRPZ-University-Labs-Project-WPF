using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using DAL.DbModels;
using DAL.DbModels.Enums;
using DAL.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DALServicesImpl.Repositories
{
    public sealed class EFCreditRepository : ICreditRepository
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public EFCreditRepository(DbContextOptions<AppDbContext> options)
        {
            _options = options;
        }

        public async Task AddOne(Credit item)
        {
            await using var context = new AppDbContext(_options);
            if (!Exists(item.Id))
            {
                var enState = await context.Credits.AddAsync(item);
                enState.State = EntityState.Added;
                await context.Transactions.AddAsync(new Transaction(DateTimeOffset.Now, TransactionType.CreditOpen,
                    item.StartSum, item.ClientId, item.BankId, item.Id));
                context.Banks.Find(item.BankId).Balance -= item.StartSum;
                await context.SaveChangesAsync();
            }
        }

        public async Task<Credit> GetOne(Guid id)
        {
            await using var context = new AppDbContext(_options);
            if (Exists(id))
            {
                var credit = await context.Credits.FindAsync(id);
                return credit;
            }
            return null;
        }

        public async Task<IReadOnlyCollection<Credit>> GetAll()
        {
            await using var context = new AppDbContext(_options);
            return context.Credits.Include(d=>d.Client).ThenInclude(c=>c.User).Include(d=>d.Bank).ToList().AsReadOnly();
        }

        public async Task UpdateOne(Credit item)
        {
            await using var context = new AppDbContext(_options);
            if (Exists(item.Id))
            {
                    var enState = context.Credits.Update(item);
                    enState.State = EntityState.Modified;
                    await context.SaveChangesAsync();
            }
        }

        public async Task DeleteOne(Guid id)
        {
            await using var context = new AppDbContext(_options);
            if (Exists(id))
            {
                var credit = await context.Credits.FindAsync(id);
                var enState = context.Credits.Remove(credit);
                enState.State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
        }

        public bool Exists(Guid id)
        {
            using var context = new AppDbContext(_options);
            return context.Credits.Any(item => item.Id.Equals(id));
        }
        public Task Pay(Guid creditId, decimal sum)
        {
            throw new NotImplementedException();
        }
    }
}
