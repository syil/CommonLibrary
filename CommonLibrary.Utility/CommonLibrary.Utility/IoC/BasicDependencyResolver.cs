using CommonLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.IoC
{
    public class BasicDependencyResolver : IDependencyResolver
    {
        private Dictionary<Type, object> container;

        public BasicDependencyResolver()
        {
            this.container = new Dictionary<Type, object>();
        }

        public TContract Resolve<TContract>()
        {
            return (TContract)Resolve(typeof(TContract));
        }

        public object Resolve(Type contractType)
        {
            object resolvedValue;

            if (this.container.TryGetValue(contractType, out resolvedValue) && resolvedValue != null)
            {
                if (resolvedValue is Type)
                {
                    return Activator.CreateInstance((Type)resolvedValue);
                }
                else if (resolvedValue is Func<object>)
                {
                    return ((Func<object>)resolvedValue).Invoke();
                }
                else
                {
                    return resolvedValue;
                }
            }
            else
            {
                CommonLibraryContainer.LoggerFactory.CreateLogger(typeof(BasicDependencyResolver)).Error($"You need to register [{contractType.FullName}] first.");
                return null;
            }
        }

        public void Register<TContract, TImplementation>()
            where TImplementation : TContract
        {
            this.container[typeof(TContract)] = typeof(TImplementation);
        }

        public void Register<TContract>(Func<TContract> constructor)
        {
            this.container[typeof(TContract)] = new Func<object>(() => constructor.Invoke());
        }

        public void RegisterInstance<TContract>(TContract obj)
        {
            this.container[typeof(TContract)] = obj;
        }
    }
}
