using InDepthSearch.Core.Enums;
using InDepthSearch.Core.Models;
using InDepthSearch.Core.Services.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InDepthSearch.Core.ViewModels
{
    public class OptionsViewModel : ViewModelBase
    {
        private Thread? _th;
        public delegate OptionsViewModel Factory(Action<SearchOptions> startSearch);
        public OptionsViewModel(IDirectoryService dirService, Action<SearchOptions> startSearch)
        {
            PrecisionOCR = new(Enum.GetValues(typeof(RecognitionPrecision)).Cast<RecognitionPrecision>());
            LanguageOCR = new(Enum.GetValues(typeof(RecognitionLanguage)).Cast<RecognitionLanguage>());
            Options = new();

            // Initialize commands
            GetDirectory = ReactiveCommand.CreateFromTask(async() =>
            {
                var newDir = await dirService.ChooseDirectory();
                if (!string.IsNullOrEmpty(newDir))
                {
                    Options.Path = newDir;
                }
            });

            ReadPDF = ReactiveCommand.Create(() => 
            {
                _th = new(() => startSearch(Options));
                _th.IsBackground = true;
                _th.Start();
            });

            // Subscribe to validation values
            this.WhenAnyValue(x => x.Options.Keyword, x => x.Options.Path, x => x.Options.UseDOC, x => x.Options.UseDOCX,
                x => x.Options.UsePDF, x => x.Options.UseODT).Subscribe(x => ValidateSelection());
        }
        [Reactive]
        public SearchOptions Options { get; set; }
        [Reactive]
        public ObservableCollection<RecognitionPrecision> PrecisionOCR { get; set; }
        [Reactive]
        public ObservableCollection<RecognitionLanguage> LanguageOCR { get; set; }
        public ReactiveCommand<Unit, Unit> ReadPDF { get; }
        public ReactiveCommand<Unit, Unit> GetDirectory { get; }
        public bool KeywordErrorVisible => string.IsNullOrWhiteSpace(Options.Keyword);
        public bool PathErrorVisible => !Directory.Exists(Options.Path);
        public bool CanExecute => !KeywordErrorVisible && !PathErrorVisible;
        public static string Logo => "avares://InDepthSearch.UI/Assets/Images/ids-logo.png";

        private void ValidateSelection()
        {
            this.RaisePropertyChanged(nameof(PathErrorVisible));
            this.RaisePropertyChanged(nameof(KeywordErrorVisible));
            this.RaisePropertyChanged(nameof(CanExecute));
        }
    }


}
