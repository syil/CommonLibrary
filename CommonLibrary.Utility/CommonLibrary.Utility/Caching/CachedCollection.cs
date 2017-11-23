using CommonLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CommonLibrary.IoC;

namespace CommonLibrary.Caching
{
    public class CachedCollection<T> : CachedObjectBase<T[]>, IDisposable
        where T : class
    {
        private static readonly ILogger logger = CommonLibraryContainer.LoggerFactory.CreateLogger(typeof(CachedCollection<T>));

        public CachedCollection(string key, Func<T[]> supplierFunction)
            : this(key, supplierFunction, TimeSpan.FromMinutes(5))
        {

        }

        public CachedCollection(string key, Func<T[]> supplierFunction, TimeSpan lifetime, bool autoRefresh = true)
            : base(key, supplierFunction, lifetime, autoRefresh)
        {

        }

        public void Dispose()
        {

        }

        protected override T[] CloneObject(T[] obj)
        {
            return obj;
        }
    }
}
