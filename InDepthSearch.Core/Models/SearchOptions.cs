using InDepthSearch.Core.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace InDepthSearch.Core.Models
{
    public class SearchOptions : ReactiveObject
    {
        public SearchOptions(string path, string keyword, RecognitionPrecision selectedPrecisionOCR, RecognitionLanguage selectedLanguageOCR, 
            bool caseSensitive, bool useOCR, bool useSubfolders, bool usePDF, bool useDOCX, bool useODT)
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
        }

        [Reactive]
        public string Path { get; set; }
        [Reactive]
        public string Keyword { get; set; }
        public RecognitionPrecision SelectedPrecisionOCR { get; set; }
        public RecognitionLanguage SelectedLanguageOCR { get; set; }
        public bool CaseSensitive { get; set; }
        public bool UseOCR { get; set; }
        public bool UseSubfolders { get; set; }
        public bool UsePDF { get; set; }
        public bool UseDOCX { get; set; }
        public bool UseODT { get; set; }
    }
}
