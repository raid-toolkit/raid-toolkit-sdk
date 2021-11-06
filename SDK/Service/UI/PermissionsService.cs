using System.Windows.Forms;

namespace Raid.Service.UI
{
    public class PermissionsService
    {
        private readonly UserData UserData;
        public PermissionsService(UserData userData)
        {
            UserData = userData;
        }

        public bool RequestPermissions(string origin)
        {
            UserSettings settings = UserData.ReadUserSettings();
            if (settings.AllowedOrigins.Contains(origin.ToLowerInvariant()))
                return true;

            var result = MessageBox.Show(new Form(), $"Would you like to give access to {origin}?", "Raid Toolkit", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.No)
                return false;

            settings.AllowedOrigins.Add(origin.ToLowerInvariant());
            UserData.WriteUserSettings(settings);
            return true;
        }
    }
}