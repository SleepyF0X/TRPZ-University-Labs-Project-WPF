using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepositories
{
    public interface IRepository<T>
    {
        Task AddOne(T item);
        Task<T> GetOne(Guid id);
        Task<IReadOnlyCollection<T>> GetAll();
        Task UpdateOne(T item);
        Task DeleteOne(Guid id);
        bool Exists(Guid id);
    }
}
