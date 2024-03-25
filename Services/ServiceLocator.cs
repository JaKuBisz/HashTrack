using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTrack.Services
{
    public class ServiceLocator
    {
        private readonly ILifetimeScope _scope;

        public ServiceLocator(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public T Resolve<T>()
        {
            return _scope.Resolve<T>();
        }
    }

}
