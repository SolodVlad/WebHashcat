using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DLL.Context
{
    public class WebHashcatDbContext : IdentityDbContext<User>
    {
        public WebHashcatDbContext(DbContextOptions<WebHashcatDbContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override async void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //HashcatResult
            builder.Entity<HashcatResult>().ToTable("SuccessfulAttackResults");
            builder.Entity<HashcatResult>().HasKey(hashcatResult => new { hashcatResult.Hash, hashcatResult.UserId });
            builder.Entity<HashcatResult>().Ignore(hashcatResult => hashcatResult.Status);
            builder.Entity<HashcatResult>().Property(hashcatResult => hashcatResult.Hash).HasColumnType("varchar").HasMaxLength(8000);
            builder.Entity<HashcatResult>().Property(hashcatResult => hashcatResult.HashType).HasColumnType("varchar").HasMaxLength(100);
            builder.Entity<HashcatResult>().Property(hashcatResult => hashcatResult.TimePassed).HasColumnType("varchar").HasMaxLength(50);
            builder.Entity<HashcatResult>().Property(hashcatResult => hashcatResult.TimeLeft).HasColumnType("varchar").HasMaxLength(50);

            //LookupTable
            builder.Entity<DataLookupTable>().ToTable("LookupTable");
            builder.Entity<DataLookupTable>().HasKey(lookupTable => lookupTable.SHA512);
            builder.Entity<DataLookupTable>().Property(lookupTable => lookupTable.MD5).HasColumnType("varchar").HasMaxLength(32).IsFixedLength();
            builder.Entity<DataLookupTable>().Property(lookupTable => lookupTable.SHA1).HasColumnType("varchar").HasMaxLength(40).IsFixedLength();
            builder.Entity<DataLookupTable>().Property(lookupTable => lookupTable.SHA256).HasColumnType("varchar").HasMaxLength(64).IsFixedLength();
            builder.Entity<DataLookupTable>().Property(lookupTable => lookupTable.SHA384).HasColumnType("varchar").HasMaxLength(96).IsFixedLength();
            builder.Entity<DataLookupTable>().Property(lookupTable => lookupTable.SHA512).HasColumnType("varchar").HasMaxLength(128).IsFixedLength();

            var md5 = MD5.Create();
            var sha1 = SHA1.Create();
            var sha256 = SHA256.Create();
            var sha384 = SHA384.Create();
            var sha512 = SHA512.Create();

            string? value;
            using var streamReader = new StreamReader("hashcat-6.2.6\\test.txt");
            while ((value = streamReader.ReadLine()) != null)
                builder.Entity<DataLookupTable>().HasData(new DataLookupTable()
                {
                    Value = value,
                    MD5 = await ComputeHashAsync(Encoding.UTF8.GetBytes(value), md5),
                    SHA1 = await ComputeHashAsync(Encoding.UTF8.GetBytes(value), sha1),
                    SHA256 = await ComputeHashAsync(Encoding.UTF8.GetBytes(value), sha256),
                    SHA384 = await ComputeHashAsync(Encoding.UTF8.GetBytes(value), sha384),
                    SHA512 = await ComputeHashAsync(Encoding.UTF8.GetBytes(value), sha512),
                });
        }

        //public DbSet<User> Users { get; set; }
        public DbSet<DataLookupTable> LookupTable { get; set; }

        private async Task<string> ComputeHashAsync(byte[] data, HashAlgorithm algorithm)
        {
            using var stream = new MemoryStream(data);
            var hashBytes = await algorithm.ComputeHashAsync(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }
    }
}
