using Windows.Storage;

namespace JsonConverter
{
    public class StorageManager : IStorageManager
    {
        private ApplicationDataContainer localSettings;

        public StorageManager()
        {
            localSettings = ApplicationData.Current.LocalSettings;
        }

        public void Set(string key, object value)
        {
            localSettings.Values[key] = value;
        }

        public object? Get(string key)
        {
            return localSettings.Values.TryGetValue(key, out object? value) ? value : null;
        }
    }

}
