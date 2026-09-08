using Drawing = System.Drawing;
using Forms = System.Windows.Forms;

namespace MoveBreak.Services;
public sealed class NotificationService : IDisposable
{
    private readonly Forms.NotifyIcon _icon;
    public event Action? ShowRequested; public event Action? PauseRequested; public event Action? ExitRequested;
    public NotificationService()
    {
        var menu = new Forms.ContextMenuStrip();
        menu.Items.Add("باز کردن MoveBreak", null, (_,_) => ShowRequested?.Invoke());
        menu.Items.Add("توقف / ادامه", null, (_,_) => PauseRequested?.Invoke());
        menu.Items.Add("خروج", null, (_,_) => ExitRequested?.Invoke());
        _icon = new Forms.NotifyIcon { Icon = Drawing.SystemIcons.Information, Text = "MoveBreak", Visible = true, ContextMenuStrip = menu };
        _icon.DoubleClick += (_,_) => ShowRequested?.Invoke();
    }
    public void ShowBreak(string title, string text, bool sound)
    {
        _icon.BalloonTipTitle = title; _icon.BalloonTipText = text; _icon.BalloonTipIcon = Forms.ToolTipIcon.Info;
        _icon.ShowBalloonTip(8000); if (sound) System.Media.SystemSounds.Asterisk.Play();
    }
    public void Dispose() { _icon.Visible = false; _icon.Dispose(); }
}
