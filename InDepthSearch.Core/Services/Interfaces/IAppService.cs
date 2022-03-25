using InDepthSearch.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDepthSearch.Core.Services.Interfaces
{
    public interface IAppService
    {
        public string GetVersion();
        public string GetCurrentLanguage();
        public string GetSearchStatus(SearchStatus ss = SearchStatus.Unknown);
        public string GetSearchInfo(SearchInfo si = SearchInfo.Unknown);
        public string GetSecondsString();
        public void ChangeLanguage();
    }
}
