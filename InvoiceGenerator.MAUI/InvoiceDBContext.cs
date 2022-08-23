using InvoiceGenerator.MAUI.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceGenerator.MAUI
{
  public class InvoiceDBContext : DbContext
  {
    public DbSet<Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlite("Data Source=InvoiceGenerator.db;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Customer>().ToTable("Customers");
    }
  }
}
