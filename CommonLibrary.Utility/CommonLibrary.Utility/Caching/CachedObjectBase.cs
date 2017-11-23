using CommonLibrary.IoC;
using CommonLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Caching
{
    public abstract class CachedObjectBase<T> : ICachedObject<T>
        where T : class
    {
        private static readonly ILogger logger = CommonLibraryContainer.LoggerFactory.CreateLogger(typeof(CachedObjectBase<T>));
        protected static readonly object readLock = new object();
        protected static readonly object refreshLock = new object();

        protected ICacheProvider cacheProvider;
        protected Func<T> supplierFunction;
        protected TimeSpan lifetime;
        protected string key;

        public string Key
        {
            get
            {
                return key;
            }
        }

        public bool AutoRefresh { get; set; }

        public bool HasCachedValue
        {
            get
            {
                var value = cacheProvider.Get<T[]>(Key);

                if (value != null)
                    return true;
                else
                    return false;
            }
        }

        public T Value
        {
            get
            {
                var value = cacheProvider.Get<T>(key);

                if (value == null)
                {
                    lock (readLock)
                    {
                        value = cacheProvider.Get<T>(key);

                        if (value == null)
                        {
                            InternalRefresh();

                            value = cacheProvider.Get<T>(key);
                        }
                    }
                }

                return CloneObject(value);
            }
        }

        public CachedObjectBase(string key, Func<T> supplierFunction, TimeSpan lifetime, bool autoRefresh = true)
        {
            this.cacheProvider = CommonLibraryContainer.CacheProvider;
            this.key = key;
            this.supplierFunction = supplierFunction;
            this.lifetime = lifetime;
            this.AutoRefresh = autoRefresh;
        }

        public void Refresh()
        {
            if (AutoRefresh)
                throw new InvalidOperationException(string.Format("Auto refreshing cache item cannot be manually refreshed. Cache key is [{0}]", key));

            InternalRefresh();
        }

        protected abstract T CloneObject(T obj);

        protected virtual T InvokeSupplierFunction()
        {
            var newValue = supplierFunction.Invoke();

            return newValue;
        }

        protected void InternalRefresh()
        {
            lock (refreshLock)
            {
                var newValue = InvokeSupplierFunction();

                if (newValue != null)
                {
                    cacheProvider.Remove(key);

                    if (AutoRefresh)
                        cacheProvider.Set(key, newValue, lifetime, InternalRefresh);
                    else
                        cacheProvider.Set(key, newValue, lifetime);
                }
                else
                {
                    logger.Error($"Supplier function for [{key}] returned null value");
                }
            }
        }
    }
}
