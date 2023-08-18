using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2CppToolkit.Runtime;

namespace Raid.Toolkit.Extensibility
{
    public interface IAccountPublicApi<T> where T : class
    {
        T GetApi();
    }

    public interface IAccountExtensionService
    {
        Task OnTick();
    }

    public interface IAccountExtension
    {
        void OnConnected(Il2CsRuntimeContext runtime);
        void OnDisconnected();
    }

    public interface IAccountExtensionFactory
    {
        IAccountExtension Create(IAccount account);
    }
}
