namespace HashTrack.Core.Models
{
    public abstract class UniqueTag<T>
    {
        public T Id { get; set; }

        protected UniqueTag()
        { }

        protected UniqueTag(T id)
        {
            Id = id;
        }

        public override bool Equals(object obj)
        {
            var other = obj as UniqueTag<T>;
            if (other == null) return false;

            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }
    }


}