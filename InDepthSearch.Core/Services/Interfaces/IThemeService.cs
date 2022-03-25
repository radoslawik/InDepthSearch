using InDepthSearch.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDepthSearch.Core.Services.Interfaces
{
    public interface IThemeService
    {
        public void ChangeTheme();
        public string GetCurrentThemeName();
    }
}
