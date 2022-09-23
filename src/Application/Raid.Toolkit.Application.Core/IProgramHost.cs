using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.Application.Core
{
    public interface IProgramHost
    {
        public Task Start(IHost host, Func<SynchronizationContext, Task> startupFunction);
    }
}
