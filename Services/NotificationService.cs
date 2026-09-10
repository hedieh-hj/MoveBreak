using Drawing = System.Drawing;
using Forms = System.Windows.Forms;

namespace MoveBreak.Services;
public sealed class NotificationService : IDisposable
{
    private readonly Forms.NotifyIcon _icon;
    private readonly Drawing.Icon _appIcon;
    private readonly Forms.ToolStripItem _openItem;
    private readonly Forms.ToolStripItem _pauseItem;
    private readonly Forms.ToolStripItem _exitItem;
    private readonly LocalizationService _localization;
    public event Action? ShowRequested; public event Action? PauseRequested; public event Action? ExitRequested;
    public NotificationService(LocalizationService localization)
    {
        _localization = localization;
        var menu = new Forms.ContextMenuStrip();
        _openItem = menu.Items.Add("", null, (_,_) => ShowRequested?.Invoke());
        _pauseItem = menu.Items.Add("", null, (_,_) => PauseRequested?.Invoke());
        _exitItem = menu.Items.Add("", null, (_,_) => ExitRequested?.Invoke());
        _appIcon = LoadApplicationIcon();
        _icon = new Forms.NotifyIcon { Icon = _appIcon, Text = "MoveBreak", Visible = true, ContextMenuStrip = menu };
        _icon.DoubleClick += (_,_) => ShowRequested?.Invoke();
        _icon.MouseClick += (_, e) => { if (e.Button == Forms.MouseButtons.Left) ShowRequested?.Invoke(); };
        _icon.BalloonTipClicked += (_, _) => ShowRequested?.Invoke();
        UpdateLanguage();
        localization.LanguageChanged += UpdateLanguage;
    }

    private static Drawing.Icon LoadApplicationIcon()
    {
        var executablePath = Environment.ProcessPath;
        if (!string.IsNullOrWhiteSpace(executablePath))
        {
            var extractedIcon = Drawing.Icon.ExtractAssociatedIcon(executablePath);
            if (extractedIcon is not null)
            {
                return extractedIcon;
            }
        }

        return (Drawing.Icon)Drawing.SystemIcons.Application.Clone();
    }

    private void UpdateLanguage() { _openItem.Text=_localization.Text("TrayOpen"); _pauseItem.Text=_localization.Text("TrayPause"); _exitItem.Text=_localization.Text("TrayExit"); }
    public void ShowBreak(string title, string text, bool sound)
    {
        _icon.BalloonTipTitle = title; _icon.BalloonTipText = text; _icon.BalloonTipIcon = Forms.ToolTipIcon.Info;
        _icon.ShowBalloonTip(8000); if (sound) System.Media.SystemSounds.Asterisk.Play();
    }
    public void Dispose()
    {
        _localization.LanguageChanged -= UpdateLanguage;
        _icon.Visible = false;
        _icon.Dispose();
        _appIcon.Dispose();
    }
}
