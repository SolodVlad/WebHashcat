using DLL.Context;
using DLL.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Linq.Expressions;

namespace DLL.Repository
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected WebHashcatDbContext _webHashcatDbContext;
        private DbSet<TEntity> _entities;

        protected DbSet<TEntity> Entities => _entities ??= _webHashcatDbContext.Set<TEntity>();

        protected BaseRepository(WebHashcatDbContext webHashcatDbContext) => _webHashcatDbContext = webHashcatDbContext;

        public virtual async Task<IReadOnlyCollection<TEntity>> GetAllAsync() => await Entities.ToListAsync().ConfigureAwait(false);

        public virtual async Task<IReadOnlyCollection<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> predicat) => await Entities.Where(predicat).ToListAsync().ConfigureAwait(false);
        
        public async Task<OperationDetail> CreateAsync(TEntity entity)
        {
            try
            {
                await Entities.AddAsync(entity).ConfigureAwait(false);
                await _webHashcatDbContext.SaveChangesAsync().ConfigureAwait(false);
                return new OperationDetail { Message = "Created" };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Create Fatal Error");
                return new OperationDetail { IsError = true, Message = "Create Fatal Error" };
            }
        }
        
        public async Task<OperationDetail> RemoveAsync(TEntity entity)
        {
            try
            {
                Entities.Remove(entity);
                await _webHashcatDbContext.SaveChangesAsync();
                return new OperationDetail { Message = "Remove" };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Remove Fatal Error");
                return new OperationDetail { IsError = true, Message = "Remove Fatal Error" };
            }
        }
    }
}