// Data/YourDbContext.cs
using Microsoft.EntityFrameworkCore;

public class YourDbContext : DbContext
{
    public YourDbContext(DbContextOptions<YourDbContext> options)
        : base(options)
    {
    }

    // No DbSet properties needed since we're using raw SQL queries
}