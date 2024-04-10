using System;

namespace HashTrack.Core
{
    public abstract class UniqueEntity
    {
        public string Id { get; set; }

        public UniqueEntity(string id)
        {
            Id = id;
        }

        public override bool Equals(object obj)
        {
            var other = obj as UniqueEntity;
            if (other == null) return false;

            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }
    }


}