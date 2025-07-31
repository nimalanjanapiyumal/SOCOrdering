using _03.QuotationService.Entities;
using Microsoft.EntityFrameworkCore;

namespace _03.QuotationService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<QuotationItem> QuotationItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Quotation>(entity =>
            {
                entity.HasKey(q => q.QuoteId);
                entity.Property(q => q.Distributor).IsRequired();
                entity.HasMany(q => q.Items)
                      .WithOne(i => i.Quotation)
                      .HasForeignKey(i => i.QuotationId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<QuotationItem>(entity =>
            {
                entity.HasKey(qi => qi.QuotationItemId);
                entity.Property(qi => qi.UnitPrice).HasPrecision(18, 2); // avoid silent truncation
                entity.Property(qi => qi.ProductId).IsRequired();
                entity.Property(qi => qi.Available).IsRequired();
            });
        }
    }
}
