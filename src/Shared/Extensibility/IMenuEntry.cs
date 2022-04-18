using System;
using System.Drawing;

namespace Raid.Toolkit.Extensibility
{
    public interface IMenuEntry
    {
        string DisplayName { get; }
        Image Image { get; }
        bool IsEnabled { get; }
        bool IsVisible { get; }
        void OnActivate();
    }
    public class MenuEntry : IMenuEntry
    {
        public event EventHandler Activate;
        public string DisplayName { get; set; }
        public Image Image { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsVisible { get; set; }
        public void OnActivate()
        {
            Activate?.Invoke(this, new());
        }
    }
}
