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

        public AppService(Language lang)
        {
            _languages = new List<ResourceInclude>()
            {
                new ResourceInclude() { Source = new Uri("avares://InDepthSearch.UI/Assets/Resources/StringsENG.xaml") },
                new ResourceInclude() { Source = new Uri("avares://InDepthSearch.UI/Assets/Resources/StringsPOL.xaml") },
                new ResourceInclude() { Source = new Uri("avares://InDepthSearch.UI/Assets/Resources/StringsFRA.xaml") },
            };
   
            currentLanguage = (int)lang;
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
            return ((Language)currentLanguage).ToString().ToUpper();
        }

        public string GetSearchStatus(SearchStatus ss)
        {
            return ss switch
            {
                SearchStatus.Ready => (string?)Avalonia.Application.Current.FindResource("StatusReady") ?? ss.ToString().ToUpper(),
                SearchStatus.Initializing => (string?)Avalonia.Application.Current.FindResource("StatusInitializing") ?? ss.ToString().ToUpper(),
                SearchStatus.Running => (string?)Avalonia.Application.Current.FindResource("StatusRunning") ?? ss.ToString().ToUpper(),
                _ => ss.ToString().ToUpper()
            };
        }

    }
}
