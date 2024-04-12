using System;
using HashTrack.Core.Enums;

namespace HashTrack.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterHandlerAttribute : RegisterKeyedAttribute
    {
        public RegisterHandlerAttribute(string key, Type serviceType = null) : base(key, LifeCycle.Transient, serviceType)
        { }
    }

}