using System;
using HashTrack.Core.Enums;

namespace HashTrack.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterServiceAttribute : Attribute
    {
        public RegisterServiceAttribute(LifeCycle lifeCycle = LifeCycle.Transient, Type serviceType = null,
            bool isOpenGeneric = false)
        {
            ServiceType = serviceType;
            LifeCycle = lifeCycle;
            IsOpenGeneric = isOpenGeneric;
        }

        public Type ServiceType { get; }
        public LifeCycle LifeCycle { get; }
        public bool IsOpenGeneric { get; }
    }
}