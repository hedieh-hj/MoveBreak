using Microsoft.EntityFrameworkCore;
using MoveBreak.Models;
using System.IO;

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

    public void EnsureCurrentSchema()
    {
        Database.OpenConnection();
        try
        {
            using var check = Database.GetDbConnection().CreateCommand();
            check.CommandText = "PRAGMA table_info('Settings');";
            using var reader = check.ExecuteReader();
            var hasLanguage = false;
            while (reader.Read())
                hasLanguage |= string.Equals(reader.GetString(1), "LanguageCode", StringComparison.OrdinalIgnoreCase);
            reader.Close();

            if (!hasLanguage)
                Database.ExecuteSqlRaw("ALTER TABLE Settings ADD COLUMN LanguageCode TEXT NOT NULL DEFAULT 'en';");
        }
        finally { Database.CloseConnection(); }
    }
}
