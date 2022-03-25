
using InDepthSearch.Core.Models;
using System.Collections.ObjectModel;

namespace InDepthSearch.Core.Managers.Interfaces
{
    public interface IResultManager
    {
        ObservableCollection<QueryResult> Results { get; }
        ResultStats Stats { get; }
        bool ItemsReady { get; set; }
        void Reinitialize();
        void AddResult(QueryResult res);
        void SetItemsReady(bool ready);
    }
}
