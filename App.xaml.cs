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
    private SingleInstanceService? _singleInstance;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _singleInstance = new SingleInstanceService();
        if (!_singleInstance.IsPrimaryInstance)
        {
            Shutdown();
            return;
        }

        var services = new ServiceCollection();
        services.AddDbContextFactory<AppDbContext>();
        services.AddSingleton<SettingsService>(); services.AddSingleton<LocalizationService>(); services.AddSingleton<ExerciseService>();
        services.AddSingleton<ActivityMonitorService>(); services.AddSingleton<NotificationService>();
        services.AddSingleton<WorkTimerService>(); services.AddSingleton<MainViewModel>(); services.AddSingleton<MainWindow>();
        _provider = services.BuildServiceProvider();
        using (var db = _provider.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext()) { db.Database.EnsureCreated(); db.EnsureCurrentSchema(); }
        var settings = _provider.GetRequiredService<SettingsService>();
        settings.LoadAsync().GetAwaiter().GetResult();
        _provider.GetRequiredService<LocalizationService>().Apply(settings.Current.LanguageCode);
        var mainWindow = _provider.GetRequiredService<MainWindow>();
        _singleInstance.ListenForActivation(() => Dispatcher.BeginInvoke(mainWindow.ShowAndActivate));
        mainWindow.ShowAndActivate();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _provider?.Dispose();
        _singleInstance?.Dispose();
        base.OnExit(e);
    }
}
