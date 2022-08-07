using System.Windows.Forms;

namespace Raid.Toolkit.Extensibility.Host
{
    public interface IWindowManager
    {
        void RegisterWindow<T>(WindowOptions options) where T : Form;
        void UnregisterWindow<T>() where T : Form;
        T CreateWindow<T>() where T : Form;
        void RestoreWindows();
        bool CanShowUI { get; internal set; }
    }
}
