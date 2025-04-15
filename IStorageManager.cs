<<<<<<< HEAD
﻿namespace JsonArchitect;

public interface IStorageManager
{
    void Set(string key, object value);
    object? Get(string key);
    Dictionary<string, object> GetAll();
    void SetAll(Dictionary<string, object> items);
}
=======
﻿using System;
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
>>>>>>> 77352be7aa5a4294ded88c5feb1fe2f71acb70fc
