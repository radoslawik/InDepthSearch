using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using InDepthSearch.Core.Services.Interfaces;
using InDepthSearch.Core.Enums;
using System;
using System.Collections.Generic;
using Avalonia.Controls.ApplicationLifetimes;

namespace InDepthSearch.UI.Services
{
    public class AppService : IAppService
    {
        private readonly List<ResourceInclude> _languages;
        private int currentLanguage = 0;
        private SearchStatus status = SearchStatus.Ready;
        private SearchInfo info = SearchInfo.Init;

        public AppService()
        {
            var basePath = "avares://InDepthSearch.UI/Assets/Resources/";
            _languages = new List<ResourceInclude>()
            {
                new() { Source = new Uri($"{basePath}StringsENG.xaml") },
                new() { Source = new Uri($"{basePath}StringsPOL.xaml") },
                new() { Source = new Uri($"{basePath}StringsFRA.xaml") },
            };
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
                SearchStatus.Ready => GetResourceString("StatusReady") ?? ss.ToString().ToUpper(),
                SearchStatus.Initializing => GetResourceString("StatusInitializing") ?? ss.ToString().ToUpper(),
                SearchStatus.Running => GetResourceString("StatusRunning") ?? ss.ToString().ToUpper(),
                _ => ss.ToString().ToUpper()
            };
        }

        public string GetSearchInfo(SearchInfo si = SearchInfo.Unknown)
        {
            if (si == SearchInfo.Unknown) si = info;
            else info = si;

            return si switch
            {
                SearchInfo.Init => GetResourceString("InfoInit") ?? si.ToString(),
                SearchInfo.Run => GetResourceString("InfoRun") ?? si.ToString(),
                SearchInfo.NoResults => GetResourceString("InfoNoResults") ?? si.ToString(),
                _ => si.ToString()
            };
        }

        public string GetSecondsString()
        {
            return GetResourceString("TimeSeconds") ?? "seconds";
        }

        private string? GetResourceString(string key)
        {
            return (string?)Avalonia.Application.Current.FindResource(key);
        }

    }
}
