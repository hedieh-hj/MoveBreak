using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MoveBreak.Data;
using MoveBreak.Models;
using MoveBreak.Services;

namespace MoveBreak.ViewModels;
public partial class MainViewModel : ObservableObject
{
    private readonly WorkTimerService _timer; private readonly ExerciseService _exercises; private readonly SettingsService _settings;
    private readonly NotificationService _notifications; private readonly IDbContextFactory<AppDbContext> _db;
    private int _historicSittingSeconds;
    [ObservableProperty] private string countdown = "45:00";
    [ObservableProperty] private string status = "در حال کار";
    [ObservableProperty] private Exercise currentExercise;
    [ObservableProperty] private int completedToday;
    [ObservableProperty] private int sittingMinutesToday;
    [ObservableProperty] private bool isBreakVisible;
    [ObservableProperty] private int reminderMinutes;
    [ObservableProperty] private int breakMinutes;
    [ObservableProperty] private bool soundEnabled;
    [ObservableProperty] private bool startWithWindows;
    [ObservableProperty] private bool eyeRuleEnabled;
    [ObservableProperty] private bool darkMode;
    public ObservableCollection<int> WeekValues { get; } = new();
    public IReadOnlyList<Exercise> Exercises => _exercises.All;
    public MainViewModel(WorkTimerService timer, ExerciseService exercises, SettingsService settings, NotificationService notifications, IDbContextFactory<AppDbContext> db)
    {
        _timer=timer; _exercises=exercises; _settings=settings; _notifications=notifications; _db=db;
        currentExercise=exercises.Next(settings.Current.EyeRuleEnabled, 0); reminderMinutes=settings.Current.ReminderMinutes; breakMinutes=settings.Current.BreakMinutes;
        soundEnabled=settings.Current.SoundEnabled; startWithWindows=settings.Current.StartWithWindows; eyeRuleEnabled=settings.Current.EyeRuleEnabled; darkMode=settings.Current.DarkMode; ApplyTheme();
        timer.Tick += UpdateClock; timer.BreakDue += OnBreakDue; notifications.PauseRequested += TogglePause; notifications.ExitRequested += () => System.Windows.Application.Current.Shutdown();
        _ = RefreshStatsAsync(); UpdateClock();
    }
    private void UpdateClock() { System.Windows.Application.Current.Dispatcher.Invoke(() => { Countdown=$"{(int)_timer.Remaining.TotalMinutes:00}:{_timer.Remaining.Seconds:00}"; Status=_timer.Status; SittingMinutesToday=(int)((_historicSittingSeconds+_timer.ActiveSeconds)/60d); }); }
    private void OnBreakDue()
    {
        if (_timer.ShouldDeferNotification) return;
        System.Windows.Application.Current.Dispatcher.Invoke(() => { CurrentExercise=_exercises.Next(EyeRuleEnabled, ReminderMinutes); IsBreakVisible=true; _timer.Pause(); _notifications.ShowBreak("وقت یک وقفه کوتاه است", CurrentExercise.Title, SoundEnabled); });
    }
    [RelayCommand] private void TogglePause() { if (_timer.IsRunning) _timer.Pause(); else _timer.Start(); UpdateClock(); }
    [RelayCommand] private void Restart() => _timer.Reset();
    [RelayCommand] private async Task CompleteAsync() { await RecordAsync(BreakResult.Completed); IsBreakVisible=false; _timer.FinishBreak(); }
    [RelayCommand] private async Task SkipAsync() { await RecordAsync(BreakResult.Skipped); IsBreakVisible=false; _timer.FinishBreak(); }
    [RelayCommand] private async Task SnoozeAsync() { await RecordAsync(BreakResult.Snoozed); IsBreakVisible=false; _timer.Snooze(5); }
    [RelayCommand] private async Task SaveSettingsAsync()
    {
        _settings.Current.ReminderMinutes=Math.Clamp(ReminderMinutes,1,180); _settings.Current.BreakMinutes=Math.Clamp(BreakMinutes,1,30);
        _settings.Current.SoundEnabled=SoundEnabled; _settings.Current.StartWithWindows=StartWithWindows; _settings.Current.EyeRuleEnabled=EyeRuleEnabled;
        _settings.Current.DarkMode=DarkMode; ApplyTheme();
        await _settings.SaveAsync(); _timer.Reset();
    }
    private void ApplyTheme()
    {
        var resources=System.Windows.Application.Current.Resources;
        resources["BackgroundBrush"]=new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(DarkMode?"#172220":"#F4F7F6"));
        resources["Surface"]=new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(DarkMode?"#22312E":"#FFFFFF"));
        resources["ForegroundBrush"]=new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(DarkMode?"#E8F1EF":"#263A37"));
        resources["MutedBrush"]=new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(DarkMode?"#A9BBB7":"#60706D"));
    }
    private async Task RecordAsync(BreakResult result)
    {
        await using var db=await _db.CreateDbContextAsync(); db.BreakRecords.Add(new(){OccurredAt=DateTime.Now,Result=result,ExerciseId=CurrentExercise.Id,SittingSeconds=_timer.ActiveSeconds}); await db.SaveChangesAsync(); await RefreshStatsAsync();
    }
    private async Task RefreshStatsAsync()
    {
        await using var db=await _db.CreateDbContextAsync(); var today=DateTime.Today;
        CompletedToday=await db.BreakRecords.CountAsync(x=>x.OccurredAt>=today && x.Result==BreakResult.Completed);
        _historicSittingSeconds=await db.BreakRecords.Where(x=>x.OccurredAt>=today).SumAsync(x=>(int?)x.SittingSeconds) ?? 0;
        var vals=new List<int>(); for(var i=6;i>=0;i--){var d=today.AddDays(-i); vals.Add(await db.BreakRecords.CountAsync(x=>x.OccurredAt>=d&&x.OccurredAt<d.AddDays(1)&&x.Result==BreakResult.Completed));}
        System.Windows.Application.Current.Dispatcher.Invoke(()=>{WeekValues.Clear();foreach(var v in vals) WeekValues.Add(v);});
    }
}
