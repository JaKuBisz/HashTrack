using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack.Core.Interfaces.Handlers
{
    public interface ISearchCompleteHandler
    {
        void HandleSearchComplete(Outlook.Search searchResult);
    }
}