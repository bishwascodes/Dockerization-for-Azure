using System.Security.Cryptography;
using CommunityToolkit.Datasync.Server.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using API.Models;

namespace API.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public const string DefaultSchema = "grocery_list";

        public DbSet<GroceryItem> GroceryItems => Set<GroceryItem>();

        public override int SaveChanges()
        {
            UpdateDatasyncMetadata();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateDatasyncMetadata();
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema(DefaultSchema);

            modelBuilder.Entity<GroceryItem>(b =>
            {
                var updatedAt = b.Property(e => e.UpdatedAt)
                    .ValueGeneratedNever()
                    .Metadata;
                updatedAt.SetBeforeSaveBehavior(PropertySaveBehavior.Save);
                updatedAt.SetAfterSaveBehavior(PropertySaveBehavior.Save);

                var version = b.Property(e => e.Version)
                    .IsConcurrencyToken()
                    .ValueGeneratedNever()
                    .Metadata;
                version.SetBeforeSaveBehavior(PropertySaveBehavior.Save);
                version.SetAfterSaveBehavior(PropertySaveBehavior.Save);
            });
        }

        private void UpdateDatasyncMetadata()
        {
            foreach (var entry in ChangeTracker.Entries<EntityTableData>())
            {
                if (entry.State is EntityState.Added or EntityState.Modified)
                {
                    if (entry.Entity.UpdatedAt is null)
                    {
                        entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
                    }

                    if (entry.Entity.Version is null || entry.Entity.Version.Length == 0)
                    {
                        entry.Entity.Version = RandomNumberGenerator.GetBytes(16);
                    }
                }
            }
        }
    }
}
