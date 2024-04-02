using HashTrack.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTrack.Helpers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterServiceAttribute : Attribute
    {
        public Type ServiceType { get; }
        public LifeCycle LifeCycle { get; }

        public RegisterServiceAttribute(Type serviceType, LifeCycle lifeCycle = LifeCycle.Transient)
        {
            ServiceType = serviceType;
            LifeCycle = lifeCycle;
        }
    }

}
