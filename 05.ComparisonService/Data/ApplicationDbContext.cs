using _05.ComparisonService.Entities;
using Microsoft.EntityFrameworkCore;

namespace _05.ComparisonService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Selection> Selections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Selection>(entity =>
            {
                entity.HasKey(s => s.SelectionId);
                entity.Property(s => s.UnitPrice).HasPrecision(18, 2);
                entity.Property(s => s.Distributor).IsRequired();
            });
        }
    }
}
