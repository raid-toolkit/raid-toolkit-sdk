using System;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host
{
    public interface IApplicationHost : IDisposable
    {
        public Task<int> Run(IRunArguments args);
    }
}