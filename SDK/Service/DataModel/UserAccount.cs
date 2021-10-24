using System.Collections.Generic;

namespace Raid.Service.DataModel
{
    public class UserAccount : IModelDataSource
    {
        private Dictionary<string, object> Data = new();
        private string UserId;
        private UserData UserData;

        public UserAccount(string userId, UserData userData)
        {
            UserData = userData;
            UserId = userId;
        }

        public T Get<T>(string key) where T : class
        {
            if (!Data.TryGetValue(key, out object value))
            {
                value = UserData.ReadAccountData<T>(UserId, key);
                Data.Add(key, value);
            }
            return (T)value;
        }

        public void Set<T>(string key, T value) where T : class
        {
            Data[key] = value;
            UserData.WriteAccountData<T>(UserId, key, value);
        }
    }
}
