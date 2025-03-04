using Windows.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonConverter
{
    public class StorageManager : IStorageManager
    {
        private ApplicationDataContainer localSettings;

        public StorageManager()
        {
            localSettings = ApplicationData.Current.LocalSettings;
        }

        public void Save(string key, object value)
        {
            localSettings.Values[key] = value;
        }

        public object? Get(string key)
        {
            return localSettings.Values.TryGetValue(key, out object? value) ? value : null;
        }
    }

}
