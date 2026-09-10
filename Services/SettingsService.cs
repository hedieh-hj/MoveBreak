using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using MoveBreak.Data;
using MoveBreak.Models;

namespace MoveBreak.Services;
public sealed class SettingsService(IDbContextFactory<AppDbContext> factory)
{
    private readonly SemaphoreSlim _saveLock = new(1, 1);
    public AppSettings Current { get; private set; } = new();
    public async Task LoadAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        Current = await db.Settings.AsNoTracking().SingleOrDefaultAsync(x => x.Id == 1) ?? new AppSettings { Id = 1 };
    }

    public async Task SaveAsync()
    {
        await _saveLock.WaitAsync();
        try
        {
            await using var db = await factory.CreateDbContextAsync();
            var stored = await db.Settings.SingleOrDefaultAsync(x => x.Id == 1);
            if (stored is null)
                db.Settings.Add(Current);
            else
                db.Entry(stored).CurrentValues.SetValues(Current);
            await db.SaveChangesAsync();

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                if (Current.StartWithWindows)
                    key?.SetValue("MoveBreak", $"\"{Environment.ProcessPath}\"");
                else
                    key?.DeleteValue("MoveBreak", false);
            }
            catch (UnauthorizedAccessException)
            {
                // Settings remain saved even when Windows denies startup-registration access.
            }
        }
        finally { _saveLock.Release(); }
    }
}
