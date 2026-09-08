using System.Globalization;
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
        InitializeComponent(); DataContext=vm; notifications.ShowRequested += ()=>Dispatcher.Invoke(()=>{Show();WindowState=WindowState.Normal;Activate();}); notifications.ExitRequested += ()=>_exit=true;
    }
    protected override void OnClosing(System.ComponentModel.CancelEventArgs e) { if(!_exit){e.Cancel=true;Hide();} base.OnClosing(e); }
}
