using Autofac;

namespace HashTrack.IoC
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