using DLL.Context;
using Microsoft.EntityFrameworkCore;

namespace DLLTest.Repository.Factory
{
    internal class ContextSingleton
    {
        private static readonly Lazy<WebHashcatDbContext> _context = new Lazy<WebHashcatDbContext>(() => new WebHashcatDbContext(new DbContextOptionsBuilder<WebHashcatDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options));

        public static WebHashcatDbContext Context { get { return _context.Value; } }
    }
}
