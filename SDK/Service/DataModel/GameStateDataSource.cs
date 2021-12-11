using System;

namespace Raid.Service
{
    public class GameStateDataSource : IModelDataSource
    {
        public DateTime LastUpdated { get; private set; }

        public T Get<T>(string key) where T : class
        {
            throw new NotImplementedException();
        }

        public T Read<T>(string key) where T : class
        {
            throw new NotImplementedException();
        }
    }
}