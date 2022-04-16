using Raid.Toolkit.Extensibility;
using System.Drawing;
using System.Windows.Forms;

namespace Raid.Toolkit.UI
{
    public partial class InstallExtensionDialog : Form
    {
        public InstallExtensionDialog(ExtensionBundle bundle)
        {
            InitializeComponent();
            pictureBox1.Image = SystemIcons.Warning.ToBitmap();
            extensionNameLabel.Text = bundle.Manifest.DisplayName;
            extensionDescriptionLabel.Text = bundle.Manifest.Description;
        }
    }
}
