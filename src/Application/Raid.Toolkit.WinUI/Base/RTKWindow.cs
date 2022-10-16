using Microsoft.UI.Xaml;

using WinUIEx;

namespace Raid.Toolkit.UI.WinUI.Base
{
    public abstract class RTKWindow : WindowEx
    {
        private static readonly EmbeddedIconId AppIcon = new(0);

        protected RTKWindow()
        {
            this.SetIcon(AppIcon.Value);
            VisibilityChanged += RTKWindow_VisibilityChanged;
        }

        private void RTKWindow_VisibilityChanged(object sender, WindowVisibilityChangedEventArgs args)
        {
            if (args.Visible)
                HandleStartPosition();
        }

        private WindowStartPosition _startPosition = WindowStartPosition.WindowsDefaultLocation;

        public WindowStartPosition StartPosition
        {
            get => _startPosition;
            set
            {
                _startPosition = value;
            }
        }

        private void HandleStartPosition()
        {
            switch (_startPosition)
            {
                case WindowStartPosition.CenterParent:
                case WindowStartPosition.CenterScreen:
                    this.CenterOnScreen(Width, Height);
                    break;
            }
        }
    }
}
