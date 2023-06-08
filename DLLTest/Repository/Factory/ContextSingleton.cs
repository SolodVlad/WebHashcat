using DLL.Context;
using Microsoft.EntityFrameworkCore;

namespace DLLTest.Repository.Factory
{
    internal class ContextSingleton
    {
        private static readonly Lazy<HashWorkDbContext> _context = new Lazy<HashWorkDbContext>(() => new HashWorkDbContext(new DbContextOptionsBuilder<HashWorkDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options));

        public static HashWorkDbContext Context { get { return _context.Value; } }
    }
}
