using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Caching
{
    public interface ICacheProvider
    {
        T Get<T>(string key) where T : class;
        void Set<T>(string key, T value, TimeSpan lifetime, Action endOfLifeCallback = null) where T : class;
        void Remove(string key);
        void Clear();
    }
}
