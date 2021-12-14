using System;

namespace Raid.DataServices
{
    public class DataStorageUpdatedEventArgs : EventArgs
    {
        public string Key { get; }
        public object Value { get; }
        public DataStorageUpdatedEventArgs(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }
    public interface IDataStorage : IDataReader
    {
        event EventHandler<DataStorageUpdatedEventArgs> Updated;
        void SetContext(IDataContext context);
        bool Write<T>(string key, T value) where T : class;
    }
}
