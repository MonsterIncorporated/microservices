using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<GeneratedNumber> GeneratedNumbers { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}