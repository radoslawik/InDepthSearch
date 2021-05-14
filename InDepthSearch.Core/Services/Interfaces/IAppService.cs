using InDepthSearch.Core.Types;
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
        public string GetSearchStatus(SearchStatus ss);
        public void ChangeLanguage();
    }
}
