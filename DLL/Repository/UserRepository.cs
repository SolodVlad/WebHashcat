using DLL.Context;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DLL.Repository
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(WebHashcatDbContext webHashcatDbContext) : base(webHashcatDbContext) { }

        public override async Task<IReadOnlyCollection<User>> GetAllAsync() => await Entities.Include(user => user.HashCrackInfos).ToListAsync().ConfigureAwait(false);

        public override async Task<IReadOnlyCollection<User>> FindByConditionAsync(Expression<Func<User, bool>> predicat) => await Entities.Include(user => user.HashCrackInfos).Where(predicat).ToListAsync().ConfigureAwait(false);
    }
}
