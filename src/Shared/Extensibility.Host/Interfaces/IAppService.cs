using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raid.Toolkit.Extensibility.Host;

public interface IAppService
{
	Task WaitForStop();
	void Restart(bool postUpdate, bool asAdmin = false, IWin32Window? owner = null);
	void Exit();
}
