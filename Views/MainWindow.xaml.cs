using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MoveBreak.Services;
using MoveBreak.ViewModels;

namespace MoveBreak.Views;
public sealed class HeightConverter : IValueConverter { public object Convert(object value, Type t, object p, CultureInfo c)=>Math.Max(5, System.Convert.ToInt32(value)*18); public object ConvertBack(object v,Type t,object p,CultureInfo c)=>throw new NotSupportedException(); }
public sealed class BoolToVisibilityConverter : IValueConverter { public object Convert(object value,Type t,object p,CultureInfo c)=>(bool)value?Visibility.Visible:Visibility.Collapsed; public object ConvertBack(object v,Type t,object p,CultureInfo c)=>throw new NotSupportedException(); }
public partial class MainWindow : Window
{
    private bool _exit;
    public MainWindow(MainViewModel vm, NotificationService notifications)
    {
        InitializeComponent();
        DataContext = vm;
        vm.BreakPresentationRequested += () => Dispatcher.Invoke(ShowBreakPrompt);
        notifications.ShowRequested += () => Dispatcher.Invoke(ShowAndActivate);
        notifications.ExitRequested += () => Dispatcher.Invoke(() =>
        {
            _exit = true;
            System.Windows.Application.Current.Shutdown();
        });
    }

    public void ShowAndActivate()
    {
        Topmost = true;
        ShowInTaskbar = true;
        if (!IsVisible) Show();
        if (WindowState == WindowState.Minimized) WindowState = WindowState.Normal;
        Activate();
        Focus();
    }

    private void ShowBreakPrompt()
    {
        WindowState = WindowState.Normal;
        ShowAndActivate();
        UpdateLayout();
        var workArea = SystemParameters.WorkArea;
        Left = workArea.Left + Math.Max(0, (workArea.Width - ActualWidth) / 2);
        Top = workArea.Top + Math.Max(0, (workArea.Height - ActualHeight) / 2);
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        if (!_exit)
        {
            e.Cancel = true;
            ShowInTaskbar = false;
            Hide();
        }
        base.OnClosing(e);
    }
}
