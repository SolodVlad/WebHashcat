using DLL.Repository;
using Domain.Models;
using System.Linq.Expressions;

namespace BLL.Services
{
    public class LookupTableService
    {
        private readonly LookupTableRepository _lookupTableRepository;

        public LookupTableService(LookupTableRepository lookupTableRepository) => _lookupTableRepository = lookupTableRepository;

        public async Task<IEnumerable<DataLookupTable>> FindAsync(Expression<Func<DataLookupTable, bool>> expression) => await _lookupTableRepository.FindByConditionAsync(expression);

        public async Task AddAsync(DataLookupTable lookupTable) => await _lookupTableRepository.CreateAsync(lookupTable);

        public async Task<DataLookupTable> GetAsync(string SHA512) => (await _lookupTableRepository.FindByConditionAsync(lookupTable => lookupTable.SHA512 == SHA512)).First();

        public async Task<IEnumerable<DataLookupTable>> GetAllAsync() => await _lookupTableRepository.GetAllAsync();

        public async Task RemoveAsync(string SHA512) => await _lookupTableRepository.RemoveAsync(await GetAsync(SHA512));
    }
}
