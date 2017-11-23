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
        private Dictionary<Type, object> instanceContainer;
        private Dictionary<Type, Delegate> instanceContainer;
        private Dictionary<Type, object> instanceContainer;

        public BasicDependencyResolver()
        {
            this.instanceContainer = new Dictionary<Type, object>();
        }

        public TContract Resolve<TContract>()
        {
            return (TContract)Resolve(typeof(TContract));
        }

        public object Resolve(Type contractType)
        {
            object resolvedValue;

            if (this.instanceContainer.TryGetValue(contractType, out resolvedValue) && resolvedValue != null)
            {
                if (resolvedValue is Type)
                {
                    return Activator.CreateInstance((Type)resolvedValue);
                }
                else if (resolvedValue.GetType())
                {
                    return ((Delegate)resolvedValue);
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
            this.instanceContainer[typeof(TContract)] = typeof(TImplementation);
        }

        public void Register<TContract>(Func<TContract> constructor)
        {
            this.instanceContainer[typeof(TContract)] = constructor;
        }

        public void RegisterInstance<TContract>(TContract obj)
        {
            this.instanceContainer[typeof(TContract)] = obj;
        }

        private bool CheckIfExists(Type type)
        {

        }
    }
}
