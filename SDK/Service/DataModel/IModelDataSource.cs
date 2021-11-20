using System;

namespace Raid.Service
{
    public interface IModelDataSource
    {
        DateTime LastUpdated { get; }
        T Read<T>(string key) where T : class;
        T Get<T>(string key) where T : class;
    }
}
