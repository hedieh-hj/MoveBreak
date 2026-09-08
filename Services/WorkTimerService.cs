using System.Windows.Threading;
using MoveBreak.Models;

namespace MoveBreak.Services;
public sealed class WorkTimerService
{
    private readonly DispatcherTimer _clock = new() { Interval = TimeSpan.FromSeconds(1) };
    private readonly ActivityMonitorService _activity; private readonly SettingsService _settings;
    private int _activeSeconds; private int _snoozeSeconds; private bool _systemPaused;
    public bool IsRunning { get; private set; } = true;
    public TimeSpan Remaining => TimeSpan.FromSeconds(Math.Max(0, (_snoozeSeconds > 0 ? _snoozeSeconds : _settings.Current.ReminderMinutes * 60) - _activeSeconds));
    public int ActiveSeconds => _activeSeconds;
    public bool ShouldDeferNotification => _activity.IsFullScreen();
    public event Action? Tick; public event Action? BreakDue;
    public WorkTimerService(ActivityMonitorService activity, SettingsService settings) { _activity=activity; _settings=settings; _clock.Tick += OnTick; _clock.Start(); }
    private void OnTick(object? s, EventArgs e)
    {
        _systemPaused = _activity.ShouldPause(_settings.Current.IdleThresholdMinutes);
        if (IsRunning && !_systemPaused) _activeSeconds++;
        Tick?.Invoke(); if (IsRunning && Remaining == TimeSpan.Zero) BreakDue?.Invoke();
    }
    public void Start() => IsRunning = true; public void Pause() => IsRunning = false;
    public void Reset() { _activeSeconds=0; _snoozeSeconds=0; IsRunning=true; Tick?.Invoke(); }
    public void Snooze(int minutes) { _activeSeconds=0; _snoozeSeconds=minutes*60; IsRunning=true; Tick?.Invoke(); }
    public void FinishBreak() { _activeSeconds=0; _snoozeSeconds=0; IsRunning=true; Tick?.Invoke(); }
    public string Status => _systemPaused ? "متوقف به‌دلیل عدم فعالیت" : IsRunning ? "در حال کار" : "متوقف‌شده";
}
