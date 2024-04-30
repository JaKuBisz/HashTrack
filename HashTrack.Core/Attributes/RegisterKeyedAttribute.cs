using System;
using HashTrack.Core.Enums;

namespace HashTrack.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterKeyedAttribute : RegisterServiceAttribute
    {
        public RegisterKeyedAttribute(string key, LifeCycle lifeCycle = LifeCycle.Transient, Type serviceType = null)
            : base(lifeCycle, serviceType)
        {
            Key = key;
        }

        public string Key { get; set; }
    }
}