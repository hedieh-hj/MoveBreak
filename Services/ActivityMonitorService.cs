using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace MoveBreak.Services;
public sealed class ActivityMonitorService : IDisposable
{
    [StructLayout(LayoutKind.Sequential)] private struct LASTINPUTINFO { public uint cbSize; public uint dwTime; }
    [DllImport("user32.dll")] private static extern bool GetLastInputInfo(ref LASTINPUTINFO info);
    [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);
    [DllImport("dwmapi.dll")] private static extern int DwmGetWindowAttribute(IntPtr hWnd, int attribute, out RECT value, int valueSize);
    private const int DwmwaExtendedFrameBounds = 9;
    [StructLayout(LayoutKind.Sequential)] private struct RECT { public int Left, Top, Right, Bottom; }
    public bool IsLocked { get; private set; }
    public ActivityMonitorService() { SystemEvents.SessionSwitch += OnSessionSwitch; }
    private void OnSessionSwitch(object sender, SessionSwitchEventArgs e) => IsLocked = e.Reason is SessionSwitchReason.SessionLock or SessionSwitchReason.SessionLogoff;
    public bool IsIdle(TimeSpan threshold)
    {
        var i = new LASTINPUTINFO { cbSize = (uint)Marshal.SizeOf<LASTINPUTINFO>() };
        return GetLastInputInfo(ref i) && TimeSpan.FromMilliseconds(unchecked((uint)Environment.TickCount - i.dwTime)) >= threshold;
    }
    public bool IsFullScreen()
    {
        var h = GetForegroundWindow();
        if (h == IntPtr.Zero) return false;
        var result = DwmGetWindowAttribute(h, DwmwaExtendedFrameBounds, out var r, Marshal.SizeOf<RECT>());
        if (result != 0 && !GetWindowRect(h, out r)) return false;
        var screen = System.Windows.Forms.Screen.FromHandle(h).Bounds;
        const int tolerance = 2;
        return Math.Abs(r.Left - screen.Left) <= tolerance
            && Math.Abs(r.Top - screen.Top) <= tolerance
            && Math.Abs(r.Right - screen.Right) <= tolerance
            && Math.Abs(r.Bottom - screen.Bottom) <= tolerance;
    }
    public bool ShouldPause(int idleMinutes) => IsLocked || IsIdle(TimeSpan.FromMinutes(idleMinutes));
    public void Dispose() => SystemEvents.SessionSwitch -= OnSessionSwitch;
}
