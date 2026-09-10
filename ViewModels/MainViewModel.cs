using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MoveBreak.Data;
using MoveBreak.Models;
using MoveBreak.Services;
using WpfApplication = System.Windows.Application;

namespace MoveBreak.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly WorkTimerService _timer;
    private readonly ExerciseService _exerciseService;
    private readonly SettingsService _settings;
    private readonly LocalizationService _localization;
    private readonly NotificationService _notifications;
    private readonly IDbContextFactory<AppDbContext> _db;
    private Exercise _currentExerciseModel;
    private int _historicSittingSeconds;

    [ObservableProperty] private string countdown = "45:00";
    [ObservableProperty] private string status = "Working";
    [ObservableProperty] private ExerciseDisplayItem currentExercise = null!;
    [ObservableProperty] private int completedToday;
    [ObservableProperty] private int sittingMinutesToday;
    [ObservableProperty] private bool isBreakVisible;
    [ObservableProperty] private int reminderMinutes;
    [ObservableProperty] private int breakMinutes;
    [ObservableProperty] private bool soundEnabled;
    [ObservableProperty] private bool startWithWindows;
    [ObservableProperty] private bool eyeRuleEnabled;
    [ObservableProperty] private bool darkMode;
    [ObservableProperty] private string selectedLanguageCode;
    [ObservableProperty] private string settingsMessage = "";

    public ObservableCollection<int> WeekValues { get; } = new();
    public ObservableCollection<ExerciseDisplayItem> Exercises { get; } = new();
    public IReadOnlyList<LanguageOption> Languages { get; } =
    [
        new("en", "English"),
        new("fa", "فارسی")
    ];
    public System.Windows.FlowDirection LayoutDirection => _localization.IsPersian
        ? System.Windows.FlowDirection.RightToLeft
        : System.Windows.FlowDirection.LeftToRight;

    public MainViewModel(
        WorkTimerService timer,
        ExerciseService exerciseService,
        SettingsService settings,
        LocalizationService localization,
        NotificationService notifications,
        IDbContextFactory<AppDbContext> db)
    {
        _timer = timer;
        _exerciseService = exerciseService;
        _settings = settings;
        _localization = localization;
        _notifications = notifications;
        _db = db;

        _currentExerciseModel = exerciseService.Next(settings.Current.EyeRuleEnabled, 0);
        reminderMinutes = settings.Current.ReminderMinutes;
        breakMinutes = settings.Current.BreakMinutes;
        soundEnabled = settings.Current.SoundEnabled;
        startWithWindows = settings.Current.StartWithWindows;
        eyeRuleEnabled = settings.Current.EyeRuleEnabled;
        darkMode = settings.Current.DarkMode;
        selectedLanguageCode = settings.Current.LanguageCode == "fa" ? "fa" : "en";

        ApplyTheme();
        RefreshLocalizedContent();
        timer.Tick += UpdateClock;
        timer.BreakDue += OnBreakDue;
        localization.LanguageChanged += RefreshLocalizedContent;
        notifications.PauseRequested += TogglePause;
        notifications.ExitRequested += () => WpfApplication.Current.Shutdown();
        _ = RefreshStatsAsync();
        UpdateClock();
    }

    partial void OnSelectedLanguageCodeChanged(string value)
    {
        if (value is not ("en" or "fa") || value == _localization.CurrentLanguage) return;
        _settings.Current.LanguageCode = value;
        _localization.Apply(value);
        _ = _settings.SaveAsync();
    }

    partial void OnDarkModeChanged(bool value)
    {
        _settings.Current.DarkMode = value;
        ApplyTheme();
        _ = PersistDarkModeAsync();
    }

    private async Task PersistDarkModeAsync()
    {
        try { await _settings.SaveAsync(); }
        catch { SettingsMessage = _localization.Text("SettingsSaveFailed"); }
    }

    private void RefreshLocalizedContent()
    {
        WpfApplication.Current.Dispatcher.Invoke(() =>
        {
            Exercises.Clear();
            foreach (var exercise in _exerciseService.All)
                Exercises.Add(ToDisplayItem(exercise));
            CurrentExercise = ToDisplayItem(_currentExerciseModel);
            Status = _localization.Text(_timer.StatusKey);
            OnPropertyChanged(nameof(LayoutDirection));
        });
    }

    private ExerciseDisplayItem ToDisplayItem(Exercise exercise) => new(
        exercise.Id,
        exercise.GetTitle(_localization.CurrentLanguage),
        exercise.GetInstructions(_localization.CurrentLanguage),
        exercise.Category,
        exercise.DurationSeconds,
        exercise.ImagePath);

    private void UpdateClock() => WpfApplication.Current.Dispatcher.Invoke(() =>
    {
        Countdown = $"{(int)_timer.Remaining.TotalMinutes:00}:{_timer.Remaining.Seconds:00}";
        Status = _localization.Text(_timer.StatusKey);
        SittingMinutesToday = (int)((_historicSittingSeconds + _timer.ActiveSeconds) / 60d);
    });

    private void OnBreakDue()
    {
        if (_timer.ShouldDeferNotification) return;
        WpfApplication.Current.Dispatcher.Invoke(() =>
        {
            SelectNextExercise();
            IsBreakVisible = true;
            _timer.Pause();
            _notifications.ShowBreak(_localization.Text("BreakNotificationTitle"), CurrentExercise.Title, SoundEnabled);
        });
    }

    [RelayCommand]
    private void TogglePause()
    {
        if (_timer.IsRunning) _timer.Pause(); else _timer.Start();
        UpdateClock();
    }

    [RelayCommand] private void Restart() => _timer.Reset();
    [RelayCommand] private async Task CompleteAsync() { await RecordAsync(BreakResult.Completed); IsBreakVisible = false; _timer.FinishBreak(); }
    [RelayCommand]
    private async Task SkipAsync()
    {
        await RecordAsync(BreakResult.Skipped);
        SelectNextExercise();
        IsBreakVisible = false;
        _timer.FinishBreak();
    }
    [RelayCommand] private async Task SnoozeAsync() { await RecordAsync(BreakResult.Snoozed); IsBreakVisible = false; _timer.Snooze(5); }

    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        try
        {
            _settings.Current.ReminderMinutes = Math.Clamp(ReminderMinutes, 1, 180);
            _settings.Current.BreakMinutes = Math.Clamp(BreakMinutes, 1, 30);
            _settings.Current.SoundEnabled = SoundEnabled;
            _settings.Current.StartWithWindows = StartWithWindows;
            _settings.Current.EyeRuleEnabled = EyeRuleEnabled;
            _settings.Current.DarkMode = DarkMode;
            _settings.Current.LanguageCode = SelectedLanguageCode;
            ApplyTheme();
            await _settings.SaveAsync();
            _timer.Reset();
            SettingsMessage = _localization.Text("SettingsSaved");
        }
        catch
        {
            SettingsMessage = _localization.Text("SettingsSaveFailed");
        }
    }

    private void ApplyTheme()
    {
        var resources = WpfApplication.Current.Resources;
        resources["BackgroundBrush"] = Brush(DarkMode ? "#172220" : "#F4F7F6");
        resources["Surface"] = Brush(DarkMode ? "#22312E" : "#FFFFFF");
        resources["ForegroundBrush"] = Brush(DarkMode ? "#E8F1EF" : "#263A37");
        resources["MutedBrush"] = Brush(DarkMode ? "#A9BBB7" : "#60706D");
        resources["ControlSurfaceBrush"] = Brush(DarkMode ? "#2E403C" : "#E9EFED");
        resources["ControlBorderBrush"] = Brush(DarkMode ? "#415651" : "#D4DEDB");
    }

    private static System.Windows.Media.SolidColorBrush Brush(string color) => new(
        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color));

    private void SelectNextExercise()
    {
        _currentExerciseModel = _exerciseService.Next(EyeRuleEnabled, ReminderMinutes, _currentExerciseModel.Id);
        CurrentExercise = ToDisplayItem(_currentExerciseModel);
    }

    private async Task RecordAsync(BreakResult result)
    {
        await using var db = await _db.CreateDbContextAsync();
        db.BreakRecords.Add(new BreakRecord
        {
            OccurredAt = DateTime.Now,
            Result = result,
            ExerciseId = _currentExerciseModel.Id,
            SittingSeconds = _timer.ActiveSeconds
        });
        await db.SaveChangesAsync();
        await RefreshStatsAsync();
    }

    private async Task RefreshStatsAsync()
    {
        await using var db = await _db.CreateDbContextAsync();
        var today = DateTime.Today;
        CompletedToday = await db.BreakRecords.CountAsync(x => x.OccurredAt >= today && x.Result == BreakResult.Completed);
        _historicSittingSeconds = await db.BreakRecords.Where(x => x.OccurredAt >= today).SumAsync(x => (int?)x.SittingSeconds) ?? 0;
        var values = new List<int>();
        for (var i = 6; i >= 0; i--)
        {
            var day = today.AddDays(-i);
            values.Add(await db.BreakRecords.CountAsync(x => x.OccurredAt >= day && x.OccurredAt < day.AddDays(1) && x.Result == BreakResult.Completed));
        }
        WpfApplication.Current.Dispatcher.Invoke(() => { WeekValues.Clear(); foreach (var value in values) WeekValues.Add(value); });
    }
}
