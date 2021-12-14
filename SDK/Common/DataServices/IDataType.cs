using System.Reflection;

namespace Raid.DataServices
{
    public interface IDataType<out T>
    {
        DataTypeAttribute Attribute { get; }
    }
    public class DataTypeManager<T> : IDataType<T>
    {
        public DataTypeManager()
        {
            Attribute = typeof(T).GetCustomAttribute<DataTypeAttribute>();
        }
        public DataTypeAttribute Attribute { get; }
    }
}