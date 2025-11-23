using Entities;
using Microsoft.EntityFrameworkCore;

namespace EfcRepositories;

public class AppContext : DbContext
{
    // Tables In The DB
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        // We Might Use Posgres In The Future
        //optionsBuilder.UseNpgsql(
        //"Host=localhost;Port=XXXX;Database=postgres;Username=postgres;Password=XXXX"
        //);
        // Until Then We Are Using Sqlite
        optionsBuilder.UseSqlite(
            "Data Source=..\\EfcRepositories\\app.db"
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("forum");
    }
}