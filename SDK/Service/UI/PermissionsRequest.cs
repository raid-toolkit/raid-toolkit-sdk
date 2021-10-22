using System.Windows.Forms;

namespace Raid.Service.UI
{
    public static class PermissionsRequest
    {
        public static bool RequestPermissions(string origin)
        {
            var result = MessageBox.Show($"Would you like to give access to {origin}?", "Raid Toolkit", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            return result == DialogResult.Yes;
        }
    }
}