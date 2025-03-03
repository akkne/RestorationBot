namespace RestorationBot.Database.DataContext;

using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Models;

public sealed class ApplicationDbContext : DbContext
{
    [SuppressMessage("ReSharper.DPA", "DPA0009: High execution time of DB command", MessageId = "time: 1028ms")]
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.Migrate();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = "Server=localhost;Database=RestorationBot;Username=postgres;Password=1425;Port=5432;Include Error Detail=true";
    
        optionsBuilder.UseNpgsql(connectionString)
                      .EnableSensitiveDataLogging();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<TrainingReport> TrainingReports { get; set; }
    public DbSet<PainReport> PainReports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}