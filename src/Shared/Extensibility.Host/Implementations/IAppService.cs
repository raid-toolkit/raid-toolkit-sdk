using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raid.Toolkit.Extensibility.Interfaces
{
    public interface IAppService
    {
        public Task WaitForStop();
        public void Restart(bool postUpdate, bool asAdmin = false, IWin32Window? owner = null);
        public void Exit();
    }
}
