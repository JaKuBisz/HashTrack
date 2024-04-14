namespace HashTrack.Exception
{
    public class UnknownResultItemTypeException : System.Exception
    {
        public UnknownResultItemTypeException() : base()
        { }

        public UnknownResultItemTypeException(string message) : base(message)
        { }
    }
}