using CommonLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Caching
{
    public class CachedObject<T> : CachedObjectBase<T>, IDisposable
        where T : class, ICloneable
    {
        private static readonly ILogger logger = CommonLibraryContainer.LoggerFactory.CreateLogger(typeof(CachedObject<T>));

        public CachedObject(string key, Func<T> supplierFunction)
            : this (key, supplierFunction, TimeSpan.FromMinutes(5))
        {

        }

        public CachedObject(string key, Func<T> supplierFunction, TimeSpan lifetime, bool autoRefresh = true)
            :base (key, supplierFunction, lifetime, autoRefresh)
        {

        }

        public void Dispose()
        {

        }

        protected override T CloneObject(T obj)
        {
            return (T)obj.Clone();
        }
    }
}
