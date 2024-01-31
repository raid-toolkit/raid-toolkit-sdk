using WinUIEx;

namespace Raid.Toolkit.UI.WinUI.Base
{
	public abstract class RTKWindow : WindowEx
    {
        private static readonly EmbeddedIconId AppIcon = new(0);

        protected RTKWindow()
        {
            this.SetIcon(AppIcon.Value);
        }
    }
}
