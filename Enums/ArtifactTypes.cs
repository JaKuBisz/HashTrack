using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTrack.Enums
{
    [Flags]
    public enum ArtifactTypes
    {
        None = 0x0,
        Email = 0x1,
        Appointment = 0x2,
        Contact = 0x4,
        Task = 0x8
    }
}
