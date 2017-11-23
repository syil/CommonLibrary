using System;
namespace CommonLibrary.Caching
{
    public interface ICachedObject<T>
        where T : class
    {
        string Key { get; }
        T Value { get; }
        bool AutoRefresh { get; set; }
        bool HasCachedValue { get; }
        void Refresh();
    }
}
