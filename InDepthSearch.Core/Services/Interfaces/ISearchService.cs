using InDepthSearch.Core.Enums;
using InDepthSearch.Core.Models;

namespace InDepthSearch.Core.Services.Interfaces
{
    public interface ISearchService
    {
        void Search(string file, SearchOptions searchOptions);
    }
}
