<<<<<<< HEAD
﻿

namespace JsonArchitect;

public class StorageManager : IStorageManager
{
    private static Dictionary<string, object> _localSettings = new();

    public void Set(string key, object value)
    {
        _localSettings[key] = value;
    }

    public object? Get(string key)
    {
        return _localSettings.GetValueOrDefault(key);
    }

    public Dictionary<string, object> GetAll()
    {
        return _localSettings;
    }

    public void SetAll(Dictionary<string, object> items)
    {
        _localSettings = items;
        
    }
}
=======
﻿using Windows.Storage;

namespace JsonConverter
{
    public class StorageManager : IStorageManager
    {
        private ApplicationDataContainer localSettings;

        public StorageManager()
        {
            localSettings = ApplicationData.Current.LocalSettings;
            //localSettings.Values.Clear(); //clear current settings
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
>>>>>>> 77352be7aa5a4294ded88c5feb1fe2f71acb70fc
