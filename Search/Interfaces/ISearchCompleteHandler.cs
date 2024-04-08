using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack.Interfaces
{
    public interface ISearchCompleteHandler
    {
        void HandleSearchComplete(Outlook.Search searchResult);
    }
}