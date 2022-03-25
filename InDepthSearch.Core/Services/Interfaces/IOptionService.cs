using Docnet.Core.Models;
using InDepthSearch.Core.Enums;

namespace InDepthSearch.Core.Services.Interfaces
{
    public interface IOptionService
    {
        public (PageDimensions, RenderFlags, ImageExtension) TranslatePrecision(RecognitionPrecision rp);
        public string TranslateLanguage(RecognitionLanguage rl);
    }
}
