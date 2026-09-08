using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using MoveBreak.Data;
using MoveBreak.Services;
using MoveBreak.ViewModels;
using MoveBreak.Views;

namespace MoveBreak;
public partial class App : System.Windows.Application
{
    private ServiceProvider? _provider;
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var services = new ServiceCollection();
        services.AddDbContextFactory<AppDbContext>();
        services.AddSingleton<SettingsService>(); services.AddSingleton<ExerciseService>();
        services.AddSingleton<ActivityMonitorService>(); services.AddSingleton<NotificationService>();
        services.AddSingleton<WorkTimerService>(); services.AddSingleton<MainViewModel>(); services.AddSingleton<MainWindow>();
        _provider = services.BuildServiceProvider();
        using (var db = _provider.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext()) db.Database.EnsureCreated();
        _provider.GetRequiredService<SettingsService>().LoadAsync().GetAwaiter().GetResult();
        _provider.GetRequiredService<MainWindow>().Show();
    }
    protected override void OnExit(ExitEventArgs e) { _provider?.Dispose(); base.OnExit(e); }
}
