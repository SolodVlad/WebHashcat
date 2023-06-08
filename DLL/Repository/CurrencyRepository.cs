using DLL.Context;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DLL.Repository
{
    public class CurrencyRepository : BaseRepository<Currency>
    {
        public CurrencyRepository(HashWorkDbContext hashWorkDbContext) : base(hashWorkDbContext) {}

        public override async Task<IReadOnlyCollection<Currency>> GetAllAsync() => await Entities.ToListAsync().ConfigureAwait(false);

        public override async Task<IReadOnlyCollection<Currency>> FindByConditionAsync(Expression<Func<Currency, bool>> predicat) => await Entities.Where(predicat).ToListAsync().ConfigureAwait(false);
    }
}