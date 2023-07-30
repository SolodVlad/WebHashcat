using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DLL.Context
{
    public class HashWorkDbContext : IdentityDbContext<User>
    {
        public HashWorkDbContext(DbContextOptions<HashWorkDbContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override async void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Currency
            builder.Entity<Currency>().ToTable(nameof(Currency));
            builder.Entity<Currency>().HasKey(currency => currency.Code);
            builder.Entity<Currency>().Property(currency => currency.Code).HasColumnType("char").HasMaxLength(3).IsFixedLength();
            builder.Entity<Currency>().Property(currency => currency.Name).HasMaxLength(100);

            //HashCrackInfo
            builder.Entity<HashCrackInfo>().HasKey(hashCrackInfo => hashCrackInfo.Hash);
            builder.Entity<HashCrackInfo>().Property(hashCrackInfo => hashCrackInfo.Hash).HasColumnType("varchar").HasMaxLength(8000);
            builder.Entity<HashCrackInfo>().Property(hashCrackInfo => hashCrackInfo.Progress).HasColumnType("tinyint");

            //LookupTable
            builder.Entity<DataLookupTable>().ToTable("LookupTable");
            builder.Entity<DataLookupTable>().HasKey(lookupTable => lookupTable.SHA512);
            builder.Entity<DataLookupTable>().Property(lookupTable => lookupTable.MD5).HasColumnType("varchar(max)");
            builder.Entity<DataLookupTable>().Property(lookupTable => lookupTable.SHA1).HasColumnType("varchar(max)");
            builder.Entity<DataLookupTable>().Property(lookupTable => lookupTable.SHA256).HasColumnType("varchar(max)");
            builder.Entity<DataLookupTable>().Property(lookupTable => lookupTable.SHA384).HasColumnType("varchar(max)");
            builder.Entity<DataLookupTable>().Property(lookupTable => lookupTable.SHA512).HasColumnType("varchar").HasMaxLength(8000);

            var md5 = MD5.Create();
            var sha1 = SHA1.Create();
            var sha256 = SHA256.Create();
            var sha384 = SHA384.Create();
            var sha512 = SHA512.Create();

            string? password;
            using var streamReader = new StreamReader("hashcat-6.2.6\\example.dict");
            while ((password = streamReader.ReadLine()) != null)
                builder.Entity<DataLookupTable>().HasData(new DataLookupTable()
                {
                    Value = password,
                    MD5 = await ComputeHashAsync(Encoding.UTF8.GetBytes(password), md5),
                    SHA1 = await ComputeHashAsync(Encoding.UTF8.GetBytes(password), sha1),
                    SHA256 = await ComputeHashAsync(Encoding.UTF8.GetBytes(password), sha256),
                    SHA384 = await ComputeHashAsync(Encoding.UTF8.GetBytes(password), sha384),
                    SHA512 = await ComputeHashAsync(Encoding.UTF8.GetBytes(password), sha512),
                });
        }

        public DbSet<User> Users { get; set; }
        public DbSet<DataLookupTable> LookupTable { get; set; }
        public DbSet<Currency> Currencies { get; set; }

        private async Task<string> ComputeHashAsync(byte[] data, HashAlgorithm algorithm)
        {
            using var stream = new MemoryStream(data);
            var hashBytes = await algorithm.ComputeHashAsync(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }
    }
}
