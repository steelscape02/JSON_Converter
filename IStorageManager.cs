using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonConverter
{
    interface IStorageManager
    {
        void Set(string key, object value);
        object? Get(string key);
    }
}
