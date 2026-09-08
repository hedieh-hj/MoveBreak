using Microsoft.EntityFrameworkCore;
using MoveBreak.Models;

namespace MoveBreak.Data;
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<BreakRecord> BreakRecords => Set<BreakRecord>();
    public DbSet<DailySession> Sessions => Set<DailySession>();
    public DbSet<AppSettings> Settings => Set<AppSettings>();
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured) {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MoveBreak");
            Directory.CreateDirectory(folder); options.UseSqlite($"Data Source={Path.Combine(folder, "movebreak.db")}");
        }
    }
}
