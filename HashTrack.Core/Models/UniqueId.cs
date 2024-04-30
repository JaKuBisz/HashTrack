namespace HashTrack.Core.Models
{
    public abstract class UniqueId<T>
    {
        protected UniqueId()
        {
        }

        protected UniqueId(T id)
        {
            Id = id;
        }

        public T Id { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as UniqueId<T>;
            if (other == null) return false;

            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }
    }
}