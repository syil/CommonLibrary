using CommonLibrary.Caching;
using CommonLibrary.IoC;
using CommonLibrary.Utilities;

namespace CommonLibrary
{
    public static class CommonLibraryContainer
    {
        static CommonLibraryContainer()
        {
            DependencyResolver = new BasicDependencyResolver();
        }

        public static IDependencyResolver DependencyResolver { get; set; }
        public static ICacheProvider CacheProvider => DependencyResolver.Resolve<ICacheProvider>();
        public static ILoggerFactory LoggerFactory => DependencyResolver.Resolve<ILoggerFactory>();
    }
}
