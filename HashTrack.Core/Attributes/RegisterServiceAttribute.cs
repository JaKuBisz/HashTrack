using System;
using HashTrack.Core.Enums;

namespace HashTrack.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterServiceAttribute : Attribute
    {
        public Type ServiceType { get; }
        public LifeCycle LifeCycle { get; }

        public RegisterServiceAttribute(LifeCycle lifeCycle = LifeCycle.Transient, Type serviceType = null)
        {
            ServiceType = serviceType;
            LifeCycle = lifeCycle;
        }
    }

}
