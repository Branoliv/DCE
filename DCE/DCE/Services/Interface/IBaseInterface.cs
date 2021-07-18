using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DCE.Services.Interface
{
    public interface IBaseInterface<T> where T : class
    {
        Task<bool> AddAsync(T obj);
        Task<bool> UpdateAsync(T obj);
        Task<bool> DeleteAsync(Guid id);
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> ListAsync();
    }
}
