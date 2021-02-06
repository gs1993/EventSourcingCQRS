using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ReadModel.Persistence
{
    public interface IReadOnlyRepository<T> where T : IReadEntity
    {
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate);

        Task<T> Get(string id);
    }

    public interface IRepository<T> : IReadOnlyRepository<T> where T : IReadEntity
    {
        Task Insert(T entity);

        Task Update(T entity);
    }
}
