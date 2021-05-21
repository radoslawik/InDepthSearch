using InDepthSearch.Core.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace InDepthSearch.Core.Models
{
    public class SearchOptions : ReactiveObject
    {
        public SearchOptions(string path, string keyword, RecognitionPrecision selectedPrecisionOCR, RecognitionLanguage selectedLanguageOCR, 
            bool caseSensitive, bool useOCR, bool useSubfolders, bool usePDF, bool useDOCX, bool useODT, bool useDOC)
        {
            Path = path;
            Keyword = keyword;
            SelectedPrecisionOCR = selectedPrecisionOCR;
            SelectedLanguageOCR = selectedLanguageOCR;
            CaseSensitive = caseSensitive;
            UseOCR = useOCR;
            UseSubfolders = useSubfolders;
            UsePDF = usePDF;
            UseDOCX = useDOCX;
            UseODT = useODT;
            UseDOCX = useDOC;
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
