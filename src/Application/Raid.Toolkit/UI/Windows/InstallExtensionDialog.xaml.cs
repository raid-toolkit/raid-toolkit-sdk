using Raid.Toolkit.Extensibility;
using Raid.Toolkit.UI.Extensions;
using System.Drawing;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Media;

namespace Raid.Toolkit.UI.Windows
{
    /// <summary>
    /// Interaction logic for InstallExtensionDialog.xaml
    /// </summary>
    public partial class InstallExtensionDialog : Window
    {
        public static ImageSource WarningIcon => SystemIcons.Warning.ToImageSource();
        public ExtensionBundle Bundle { get; }

        public InstallExtensionDialog(ExtensionBundle bundle)
        {
            ElementHost.EnableModelessKeyboardInterop(this);
            Bundle = bundle;
            InitializeComponent();
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
