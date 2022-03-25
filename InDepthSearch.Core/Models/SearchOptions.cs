using InDepthSearch.Core.Enums;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace InDepthSearch.Core.Models
{
    public class SearchOptions : ReactiveObject
    {
        public SearchOptions()
        {
            UseOCR = true;
            UsePDF = true;
            Path = "";
            Keyword = "";
            SelectedLanguageOCR = RecognitionLanguage.Default;
            SelectedPrecisionOCR = RecognitionPrecision.Default;
        }

        [Reactive]
        public string Path { get; set; }
        [Reactive]
        public string Keyword { get; set; }
        public RecognitionPrecision SelectedPrecisionOCR { get; set; }
        public RecognitionLanguage SelectedLanguageOCR { get; set; }
        public bool CaseSensitive { get; set; }
        [Reactive]
        public bool UseOCR { get; set; }
        public bool UseSubfolders { get; set; }
        [Reactive]
        public bool UsePDF { get; set; }
        [Reactive]
        public bool UseDOCX { get; set; }
        [Reactive]
        public bool UseODT { get; set; }
        [Reactive]
        public bool UseDOC { get; set; }
    }
}
