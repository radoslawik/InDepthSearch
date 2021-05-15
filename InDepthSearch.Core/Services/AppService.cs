using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using InDepthSearch.Core.Services.Interfaces;
using InDepthSearch.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDepthSearch.Core.Services
{
    public class AppService : IAppService
    {
        private readonly List<ResourceInclude> _languages;
        private int currentLanguage;
        private SearchStatus status;
        private SearchInfo info;

        public AppService(AppLanguage lang)
        {
            _languages = new List<ResourceInclude>()
            {
                new ResourceInclude() { Source = new Uri("avares://InDepthSearch.UI/Assets/Resources/StringsENG.xaml") },
                new ResourceInclude() { Source = new Uri("avares://InDepthSearch.UI/Assets/Resources/StringsPOL.xaml") },
                new ResourceInclude() { Source = new Uri("avares://InDepthSearch.UI/Assets/Resources/StringsFRA.xaml") },
            };
   
            currentLanguage = (int)lang;
            status = SearchStatus.Ready;
            info = SearchInfo.Init;
            
        }

        public string GetVersion()
        {
            var assemblyVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            return assemblyVer != null ? assemblyVer.Major.ToString() + "." + assemblyVer.Minor.ToString() + "."
                + assemblyVer.Build.ToString() : "x.x.x";
              
        }

        public void ChangeLanguage()
        {
            currentLanguage++;
            if (currentLanguage >= _languages.Count) currentLanguage = 0;
            Avalonia.Application.Current.Resources.MergedDictionaries[0] = _languages[currentLanguage];
        }

        public string GetCurrentLanguage()
        {
            return ((AppLanguage)currentLanguage).ToString().ToUpper();
        }

        public string GetSearchStatus(SearchStatus ss = SearchStatus.Unknown)
        {
            if (ss == SearchStatus.Unknown) ss = status;
            else status = ss;

            return ss switch
            {
                SearchStatus.Ready => (string?)Avalonia.Application.Current.FindResource("StatusReady") ?? ss.ToString().ToUpper(),
                SearchStatus.Initializing => (string?)Avalonia.Application.Current.FindResource("StatusInitializing") ?? ss.ToString().ToUpper(),
                SearchStatus.Running => (string?)Avalonia.Application.Current.FindResource("StatusRunning") ?? ss.ToString().ToUpper(),
                _ => ss.ToString().ToUpper()
            };
        }

        public string GetSearchInfo(SearchInfo si = SearchInfo.Unknown)
        {
            if (si == SearchInfo.Unknown) si = info;
            else info = si;

            return si switch
            {
                SearchInfo.Init => (string?)Avalonia.Application.Current.FindResource("InfoInit") ?? si.ToString(),
                SearchInfo.Run => (string?)Avalonia.Application.Current.FindResource("InfoRun") ?? si.ToString(),
                SearchInfo.NoResults => (string?)Avalonia.Application.Current.FindResource("InfoNoResults") ?? si.ToString(),
                _ => si.ToString()
            };
        }

        public string GetSecondsString()
        {
            return (string?)Avalonia.Application.Current.FindResource("TimeSeconds") ?? "seconds";
        }

    }
}
