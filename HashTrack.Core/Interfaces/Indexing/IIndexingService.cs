using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack.Interfaces.Indexing
{
    public interface IIndexingService
    {
        void IndexSearchResults(Outlook.Search searchResult);
    }
}