using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Raid.Toolkit.Extensibility.DataServices
{
    public interface IDataStorageReaderWriter
    {
        bool TryRead<T>(string filePath, [NotNullWhen(true)] out T? value) where T : class;
        bool Write<T>(string filePath, T value) where T : class;
        IEnumerable<string> GetKeys(string filePath);
        void Flush();
    }
}
