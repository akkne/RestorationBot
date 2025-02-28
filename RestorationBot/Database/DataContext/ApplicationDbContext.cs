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

    public DbSet<User> Users { get; set; }
    public DbSet<TrainingReport> TrainingReports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}