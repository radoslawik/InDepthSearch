using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.ObjectModel;
using InDepthSearch.Core.Models;
using InDepthSearch.Core.Enums;
using System.Threading;
using InDepthSearch.Core.Services.Interfaces;
using System.Diagnostics;
using InDepthSearch.Core.Managers.Interfaces;

namespace InDepthSearch.Core.ViewModels
{
    public class MainWindowViewModel: ViewModelBase
    { 
        private readonly IThemeService _themeService;
        private readonly IAppService _infoService;
        private readonly ISearchService _searchService;

        public MainWindowViewModel(IAppService infoService, IThemeService themeService, IResultManager resultManager, ISearchService searchService, 
            ResultsViewModel.Factory resultsFactory, OptionsViewModel.Factory optionsFactory)
        {
            // Initialize services
            _themeService = themeService;
            _infoService = infoService;
            _searchService = searchService;
            ResultManager = resultManager;

            ResultsPage = resultsFactory();
            OptionsMenu = optionsFactory(StartReading);

            ChangeTheme = ReactiveCommand.Create(() =>
            {
                themeService.ChangeTheme();
                CurrentThemeName = themeService.GetCurrentThemeName();
            });
            ChangeLanguage = ReactiveCommand.Create(() =>
            {
                infoService.ChangeLanguage();
                CurrentLanguageName = infoService.GetCurrentLanguage();
                UpdateStringResources();
            });
            OpenUrl = ReactiveCommand.Create(() =>
            {
                var url = "https://github.com/radoslawik/InDepthSearch";
                using var process = Process.Start(new ProcessStartInfo
                {
                    FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? url : "open",
                    Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? $"-e {url}" : "",
                    CreateNoWindow = true,
                    UseShellExecute = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                }); 
            });

            // Initialize variables
            StatusName = infoService.GetSearchStatus(SearchStatus.Ready);
            CurrentThemeName = themeService.GetCurrentThemeName();
            CurrentLanguageName = infoService.GetCurrentLanguage();

            // Get assembly version
            AppVersion = infoService.GetVersion();
        }
        public IResultManager ResultManager { get; }
        public ResultsViewModel ResultsPage { get; }
        public OptionsViewModel OptionsMenu { get; }

        public ReactiveCommand<Unit, Unit> ChangeTheme { get; }
        public ReactiveCommand<Unit, Unit> ChangeLanguage { get; }
        public ReactiveCommand<Unit, Unit> OpenUrl { get; }

        [Reactive]
        public string AppVersion { get; set; }
        [Reactive]
        public string CurrentThemeName { get; set; }
        [Reactive]
        public string CurrentLanguageName { get; set; }
        [Reactive]
        public string StatusName { get; set; }

        private void UpdateStringResources()
        {
            CurrentThemeName = _themeService.GetCurrentThemeName();
            StatusName =  _infoService.GetSearchStatus();
            ResultsPage.UpdateResultInfo(_infoService.GetSearchInfo());
        }

        private void StartReading(SearchOptions searchOptions)
        {
            StatusName = _infoService.GetSearchStatus(SearchStatus.Initializing);
            ResultManager.Reinitialize();

            var fileCounter = 0;
            var watch = System.Diagnostics.Stopwatch.StartNew();

            if (!string.IsNullOrWhiteSpace(searchOptions.Keyword))
            {
                var allowedExtensions = new List<string>();

                if (searchOptions.UsePDF)
                    allowedExtensions.Add(".pdf");
                if (searchOptions.UseDOCX)
                    allowedExtensions.Add(".docx");
                if (searchOptions.UseDOC)
                    allowedExtensions.Add(".doc");
                if (searchOptions.UseODT)
                    allowedExtensions.Add(".odt");

                List<string> discoveredFiles = searchOptions.UseSubfolders ? 
                    Directory.GetFiles(searchOptions.Path, "*.*", SearchOption.AllDirectories)
                        .Where(file => allowedExtensions.Any(file.ToLower().EndsWith)).ToList() : 
                    Directory.GetFiles(searchOptions.Path)
                        .Where(file => allowedExtensions.Any(file.ToLower().EndsWith)).ToList();

                if (discoveredFiles == null)
                {
                    System.Diagnostics.Debug.WriteLine("No files found... ");
                    return;
                }

                StatusName = _infoService.GetSearchStatus(SearchStatus.Running);
                ResultsPage.UpdateResultInfo(_infoService.GetSearchInfo(SearchInfo.Init));
                ResultManager.Stats.FilesAnalyzed = "0/" + discoveredFiles.Count.ToString();

                foreach (var file in discoveredFiles)
                {
                    System.Diagnostics.Debug.WriteLine("Checking " + file);

                    _searchService.Search(file, searchOptions);
                    fileCounter += 1;
                    ResultManager.Stats.FilesAnalyzed = fileCounter.ToString() + "/" + discoveredFiles.Count.ToString();
                }
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            System.Diagnostics.Debug.WriteLine("Total execution " + elapsedMs);
            ResultManager.Stats.ExecutionTime = (elapsedMs / 1000.0).ToString() + " " + _infoService.GetSecondsString();
            StatusName = _infoService.GetSearchStatus(SearchStatus.Ready);
            if (!ResultManager.Results.Any())
            {
                ResultsPage.UpdateResultInfo(_infoService.GetSearchInfo(SearchInfo.NoResults));
                ResultManager.SetItemsReady(false);
            }
        }
    }

}
