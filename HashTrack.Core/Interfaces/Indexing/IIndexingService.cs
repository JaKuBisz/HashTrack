using System;

namespace HashTrack.Interfaces.Indexing
{
    public interface IIndexingService
    {
        void IndexAllArtifacts(DateTime? from = null);
    }
}