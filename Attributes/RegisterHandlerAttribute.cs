using System;
using Accessibility;
using HashTrack.Enums;
using HashTrack.Helpers;
using HashTrack.Interfaces;

namespace HashTrack.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterHandlerAttribute : RegisterServiceAttribute
    {
        public string Tag { get; set; }

        public RegisterHandlerAttribute(Type serviceType, string tag) : base(serviceType, LifeCycle.Transient)
        {
            Tag = tag;
        }
    }

}