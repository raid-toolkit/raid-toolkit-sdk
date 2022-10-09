using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Launcher
{
    public enum CheckStatus
    {
        None,
        Running,
        Fail,
        Success
    }
    public class InstallCheckEventArgs : EventArgs
    {
        private readonly InstallCheck Sender;
        public InstallCheckEventArgs(InstallCheck sender)
        {
            Sender = sender;
        }

        public void Succeed()
        {
            Sender.Dispatcher.Invoke(() =>
            {
                Sender.Status = CheckStatus.Success;
                Sender.Foreground = new SolidColorBrush(Colors.LimeGreen);
            });
        }

        public void Fail(string message)
        {
            Sender.Dispatcher.Invoke(() =>
            {
                Sender.Status = CheckStatus.Fail;
                Sender.DisplayName = message;
                Sender.Foreground = new SolidColorBrush(Colors.Red);
            });
        }
    }
    [ValueConversion(typeof(CheckStatus), typeof(string))]
    public class CheckStatusToIconFilenameConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (CheckStatus)value switch
            {
                CheckStatus.Running => "⌛",
                CheckStatus.Success => "✅",
                CheckStatus.Fail => "❌",
                _ => "",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotSupportedException();
    }
    /// <summary>
    /// Interaction logic for InstallCheck.xaml
    /// </summary>
    public partial class InstallCheck : UserControl
    {
        public InstallCheck()
        {
            InitializeComponent();
        }

        public event EventHandler<InstallCheckEventArgs>? Check;

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(string), typeof(InstallCheck), new PropertyMetadata(""));

        public CheckStatus Status
        {
            get { return (CheckStatus)GetValue(SuccessProperty); }
            set { SetValue(SuccessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SuccessProperty =
            DependencyProperty.Register(nameof(Status), typeof(CheckStatus), typeof(InstallCheck), new PropertyMetadata(CheckStatus.None));

        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayNameProperty =
            DependencyProperty.Register(nameof(DisplayName), typeof(string), typeof(InstallCheck), new PropertyMetadata(""));

        private void control_Loaded(object sender, RoutedEventArgs e)
        {
            if (Check != null)
            {
                Status = CheckStatus.Running;
                Check.Invoke(this, new(this));
            }
        }
    }
}
