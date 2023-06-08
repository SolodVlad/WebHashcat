using DLL.Context;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace DLL.Repository
{
    public class LookupTableRepository : BaseRepository<DataLookupTable>
    {
        public LookupTableRepository(HashWorkDbContext hashWorkDbContext) : base(hashWorkDbContext) { }

        public override async Task<IReadOnlyCollection<DataLookupTable>> GetAllAsync() => await Entities.ToListAsync().ConfigureAwait(false);

        public override async Task<IReadOnlyCollection<DataLookupTable>> FindByConditionAsync(Expression<Func<DataLookupTable, bool>> predicat) => await Entities.Where(predicat).ToListAsync().ConfigureAwait(false);
    }
}