using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using MoveBreak.Data;
using MoveBreak.Models;

namespace MoveBreak.Services;
public sealed class SettingsService(IDbContextFactory<AppDbContext> factory)
{
    public AppSettings Current { get; private set; } = new();
    public async Task LoadAsync() { await using var db = await factory.CreateDbContextAsync(); Current = await db.Settings.FindAsync(1) ?? new(); if (Current.Id == 0) Current.Id = 1; }
    public async Task SaveAsync()
    {
        await using var db = await factory.CreateDbContextAsync(); db.Settings.Update(Current); await db.SaveChangesAsync();
        using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
        if (Current.StartWithWindows) key?.SetValue("MoveBreak", $"\"{Environment.ProcessPath}\""); else key?.DeleteValue("MoveBreak", false);
    }
}
