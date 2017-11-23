using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.IoC
{
    public interface IDependencyResolver
    {
        TContract Resolve<TContract>();

        object Resolve(Type contractType);

        void Register<TContract, TImplementation>() where TImplementation : TContract;

        void Register<TContract>(Func<TContract> constructor);

        void RegisterInstance<TContract>(TContract obj);
    }
}
