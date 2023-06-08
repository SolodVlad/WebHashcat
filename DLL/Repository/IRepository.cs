using DLL.Infrastructure;
using System.Linq.Expressions;

namespace DLL.Repository
{
    public interface IRepository<TEntity>
    {
        Task<IReadOnlyCollection<TEntity>> GetAllAsync();
        Task<IReadOnlyCollection<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> predicat);
        Task<OperationDetail> CreateAsync(TEntity entity);
        Task<OperationDetail> RemoveAsync(TEntity entity);
    }
}