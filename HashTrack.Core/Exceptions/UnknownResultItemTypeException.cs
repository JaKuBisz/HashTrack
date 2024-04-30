namespace HashTrack.Exception
{
    public class UnknownResultItemTypeException : System.Exception
    {
        public UnknownResultItemTypeException()
        {
        }

        public UnknownResultItemTypeException(string message) : base(message)
        {
        }
    }
}