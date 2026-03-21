using System.Security.Cryptography;
using CommunityToolkit.Datasync.Server.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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
        }

        private void UpdateDatasyncMetadata()
        {
            foreach (var entry in ChangeTracker.Entries<EntityTableData>())
            {
                if (entry.State is EntityState.Added or EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
                    entry.Entity.Version = RandomNumberGenerator.GetBytes(16);
                }
            }
        }
    }
}
