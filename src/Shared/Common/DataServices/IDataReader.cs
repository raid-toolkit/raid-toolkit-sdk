using System.Collections.Generic;

namespace Raid.DataServices
{
    public interface IDataReader
    {
        IEnumerable<string> Keys { get; }
        bool TryRead<T>(string key, out T value) where T : class;
    }
}
