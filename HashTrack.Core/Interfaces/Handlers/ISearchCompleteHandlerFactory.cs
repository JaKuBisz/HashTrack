namespace HashTrack.Core.Interfaces.Handlers
{
    public interface ISearchCompleteHandlerFactory
    {
        void HandleSearchCompleted(Microsoft.Office.Interop.Outlook.Search searchObject);
    }
}