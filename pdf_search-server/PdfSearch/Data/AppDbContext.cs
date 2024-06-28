using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace pdf_search.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options) {}

    public DbSet<Submissions> Submissions {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
         modelBuilder.Entity<Submissions>()
            .HasKey(s => new { s.batch, s.name });
        base.OnModelCreating(modelBuilder);
    }
}