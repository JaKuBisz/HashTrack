using System;

namespace HashTrack.Core.Enums
{
    [Flags]
    public enum ArtifactTypes
    {
        None = 0x0,
        Email = 0x1,
        Appointment = 0x2,
        Contact = 0x4,
        Task = 0x8,
        All = Email | Appointment | Contact | Task
    }
}
