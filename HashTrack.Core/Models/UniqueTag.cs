namespace HashTrack.Core.Models
{
    public abstract class UniqueTag
    {
        public string Tag { get; set; }

        protected UniqueTag()
        { }

        protected UniqueTag(string tag)
        {
            Tag = tag;
        }

        public override bool Equals(object obj)
        {
            var other = obj as UniqueTag;
            if (other == null) return false;

            return Tag == other.Tag;
        }

        public override int GetHashCode()
        {
            return Tag?.GetHashCode() ?? 0;
        }
    }


}