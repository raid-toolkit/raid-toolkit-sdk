using System;

namespace Raid.Toolkit.Extensibility.DataServices
{
    public class DataStorageUpdatedEventArgs : EventArgs
    {
        public IDataContext Context { get; }
        public string Key { get; }
        public object Value { get; }
        public DataStorageUpdatedEventArgs(IDataContext context, string key, object value)
        {
            Context = context;
            Key = key;
            Value = value;
        }
    }
    public interface IDataStorage : IDataReader
    {
        event EventHandler<DataStorageUpdatedEventArgs> Updated;
        bool Write<T>(IDataContext context, string key, T value) where T : class;
    }
}
