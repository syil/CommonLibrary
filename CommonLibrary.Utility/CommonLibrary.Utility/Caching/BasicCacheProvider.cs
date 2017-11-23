using CommonLibrary.Caching;
using CommonLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonLibrary.Caching
{
    public class BasicCacheProvider : ICacheProvider, IDisposable
    {
        private static readonly ILogger logger = CommonLibraryContainer.LoggerFactory.CreateLogger(typeof(BasicCacheProvider));

        private Dictionary<string, CacheValue> collection;
        private object writeLock = new object();
        private bool lifetimeCheckerToken;
        private TimeSpan lifetimeCheckerPeriod;

        

        public BasicCacheProvider(TimeSpan? lifetimeCheckerPeriod = null)
        {
            this.collection = new Dictionary<string, CacheValue>();
            this.lifetimeCheckerToken = true;
            this.lifetimeCheckerPeriod = lifetimeCheckerPeriod.GetValueOrDefault(TimeSpan.FromSeconds(30));

            CheckLifetime();
        }

        public void Clear()
        {
            collection.Clear();
        }

        public T Get<T>(string key) 
            where T : class
        {
            CacheValue value;
            if (collection.TryGetValue(key, out value))
                return value.Value as T;
            else
            {
                logger.Debug($"[{key}] not found in collection");
                return null;
            }
        }

        public void Remove(string key)
        {
            collection.Remove(key);
        }

        public void Set<T>(string key, T value, TimeSpan lifetime, Action endOfLifeCallback = null) 
            where T : class
        {
            var item = new CacheValue(DateTime.Now, lifetime, endOfLifeCallback, value);

            lock (writeLock)
            {
                if (collection.ContainsKey(key))
                    collection[key] = item;
                else
                    collection.Add(key, item); 
            }
        }

        private async void CheckLifetime()
        {
            while (!lifetimeCheckerToken)
            {
                lock (writeLock)
                {
                    if (!lifetimeCheckerToken)
                    {
                        DateTime now = DateTime.Now;
                        var expiredItems = collection.Where(i => (now - i.Value.InsertDate) > i.Value.Lifetime).ToList();

                        foreach (var item in expiredItems)
                        {
                            collection.Remove(item.Key);

                            item.Value.EndOfLifeCallback?.Invoke();
                        } 
                    }
                }

                await Task.Delay(lifetimeCheckerPeriod);
            }
        }

        private class CacheValue
        {
            public DateTime InsertDate { get; set; }
            public TimeSpan Lifetime { get; set; }
            public Action EndOfLifeCallback { get; set; }
            public object Value { get; set; }

            public CacheValue(DateTime insertDate, TimeSpan lifetime, Action endOfLifeCallback, object value)
            {
                InsertDate = insertDate;
                Lifetime = lifetime;
                EndOfLifeCallback = endOfLifeCallback;
                Value = value;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    lifetimeCheckerToken = false;

                    if (collection != null)
                    {
                        collection.Clear();
                        collection = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
