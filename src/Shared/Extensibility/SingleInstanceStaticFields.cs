using Il2CppToolkit.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility
{
    [Size(16)]
    public struct SingleInstanceStaticFields<T>
    {
        [Offset(8)]
#pragma warning disable 649
        public T Instance;
#pragma warning restore 649
    }
}
