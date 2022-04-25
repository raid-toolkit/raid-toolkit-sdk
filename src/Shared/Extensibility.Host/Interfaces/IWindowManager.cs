using System.Windows.Forms;

namespace Raid.Toolkit.Extensibility.Host
{
    public interface IWindowManager
    {
        public void RegisterWindow<T>(WindowOptions options) where T : Form;
        public void UnregisterWindow<T>() where T : Form;
        public T CreateWindow<T>() where T : Form;
        public void RestoreWindows();
    }
}
