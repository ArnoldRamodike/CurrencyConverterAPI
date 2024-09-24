using CurrencyConverterAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace CurrencyConverterAPI.Data
{
    public class CurrencyDbContext(DbContextOptions<CurrencyDbContext> options) : DbContext(options)
    {
        public DbSet<CurrencyRate> CurrencyRates => Set<CurrencyRate>();
        public DbSet<ConversionHistory> ConversionHistories => Set<ConversionHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CurrencyRate>()
            .Property(e => e.Date)
            .HasConversion(
            v => v.ToDateTime(TimeOnly.MinValue),  // Converts DateOnly to DateTime when saving to the database
            v => DateOnly.FromDateTime(v)          // Converts DateTime to DateOnly when reading from the database
        );
            modelBuilder.Entity<ConversionHistory>()
            .Property(c => c.Date)
            .HasConversion(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v)
        );

            base.OnModelCreating(modelBuilder);
        }
    }
}