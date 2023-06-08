using DLL.Repository;
using Domain.Models;
using System.Linq.Expressions;

namespace BLL.Services
{
    public class CurrencyService
    {
        private readonly CurrencyRepository _currencyRepository;

        public CurrencyService(CurrencyRepository currencyRepository) => _currencyRepository = currencyRepository;

        public async Task<IEnumerable<Currency>> FindAsync(Expression<Func<Currency, bool>> expression) => await _currencyRepository.FindByConditionAsync(expression);

        public async Task AddAsync(Currency currency) => await _currencyRepository.CreateAsync(currency);

        public async Task<Currency> GetAsync(string code) => (await _currencyRepository.FindByConditionAsync(currency => currency.Code == code)).First();

        public async Task<IEnumerable<Currency>> GetAllAsync() => await _currencyRepository.GetAllAsync();

        public async Task RemoveAsync(string code)
        {
            //var entity = (await _currencyRepository.FindByConditionAsync(x => x.Id == id)).First();
            await _currencyRepository.RemoveAsync(await GetAsync(code));
        }
    }
}
