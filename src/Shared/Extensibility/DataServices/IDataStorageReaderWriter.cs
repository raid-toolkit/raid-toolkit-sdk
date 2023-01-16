using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility.DataServices
{
    public interface IDataStorageReaderWriter
    {
        bool TryRead<T>(string filePath, out T value) where T : class;
        bool Write<T>(string filePath, T value) where T : class;
        IEnumerable<string> GetKeys(string filePath);
    }
}
