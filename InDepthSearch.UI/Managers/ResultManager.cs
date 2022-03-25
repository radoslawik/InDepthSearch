using InDepthSearch.Core.Managers.Interfaces;
using InDepthSearch.Core.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;

namespace InDepthSearch.UI.Managers
{
    public class ResultManager : ReactiveObject, IResultManager
    {
        public ResultManager()
        {
            Results = new();
            Stats = new();
        }

        [Reactive]
        public ObservableCollection<QueryResult> Results { get; }
        [Reactive]
        public ResultStats Stats { get; set; }
        [Reactive]
        public bool ItemsReady { get; set; }

        public void Reinitialize()
        {
            Results.Clear();
            ItemsReady = false;
            Stats.FilesAnalyzed = "0/0";
            Stats.PagesAnalyzed = 0;
            Stats.ExecutionTime = "";
        }

        public void AddResult(QueryResult res) => Results.Add(res);
        public void SetItemsReady(bool ready) => ItemsReady = ready;
    }
}
