using CurrencyTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracker.Services;

public class CurrencyDbContext : DbContext
{
    public DbSet<RatesHistory> RatesHistories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=currency.db");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RatesHistory>()
            .HasIndex(r => new { r.BaseCurrency, r.TargetCurrency, r.Date })
            .IsUnique();
    }
}