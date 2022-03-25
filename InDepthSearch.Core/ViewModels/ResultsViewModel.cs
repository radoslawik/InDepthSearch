using InDepthSearch.Core.Enums;
using InDepthSearch.Core.Managers.Interfaces;
using InDepthSearch.Core.Services.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDepthSearch.Core.ViewModels
{
    public class ResultsViewModel : ViewModelBase
    {
        public delegate ResultsViewModel Factory();
        public ResultsViewModel(IResultManager resultManager, IAppService appService)
        {
            ResultManager = resultManager;
            ResultInfo = appService.GetSearchInfo(SearchInfo.Init);
            ResultManager.Results.CollectionChanged += (s, e) => Teestc();
        }

        private void Teestc()
        {
            var test = ResultManager.Results.Count;
            this.RaisePropertyChanged(nameof(ResultManager));
        }

        public IResultManager ResultManager { get; }
        [Reactive]
        public string ResultInfo { get; set; }
        public void UpdateResultInfo(string info)
        {
            ResultInfo = info;
        }
    }
}
