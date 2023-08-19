using DLL.Repository;
using Domain.Models;
using System.Linq.Expressions;

namespace BLL.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository) => _userRepository = userRepository;

        public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> expression) => await _userRepository.FindByConditionAsync(expression);

        public async Task AddAsync(User user) => await _userRepository.CreateAsync(user);

        public async Task<User> GetAsync(string id) => (await _userRepository.FindByConditionAsync(x => x.Id == id)).First();

        public async Task<IEnumerable<User>> GetAllAsync() => await _userRepository.GetAllAsync();

        public async Task RemoveAsync(string id) => await _userRepository.RemoveAsync(await GetAsync(id));
    }
}
