using Gesd.Entite;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Reflection.Emit;

using File = Gesd.Entite.File;

namespace Gesd.Api.Context
{
    public class GesdContext : DbContext
    {
        public GesdContext(DbContextOptions<GesdContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<File>()
                .HasOne(p => p.EncryptedUrlFile)
                .WithOne(a => a.File)
                .HasForeignKey<EncryptedUrlFile>(a => a.FileId);

            modelBuilder.Entity<EncryptedUrlFile>()
                .HasOne(p => p.GenerateStoredKey)
                .WithOne(a => a.EncryptedUrlFile)
                .HasForeignKey<KeyStore>(a => a.EncryptedUrlId);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<File>())
            {
                entry.Entity.LastModify = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.Now;
                }

            }
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public DbSet<File> Files { get; set; }
        public DbSet<EncryptedUrlFile> EncryptedUrlFiles { get; set; }
        public DbSet<KeyStore> KeyStores { get; set; }
    }
}
