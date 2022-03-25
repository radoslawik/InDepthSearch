using Docnet.Core.Models;
using InDepthSearch.Core.Services.Interfaces;
using InDepthSearch.Core.Enums;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDepthSearch.Core.Services
{
    public class OptionService : IOptionService
    {
        public (PageDimensions, RenderFlags, ImageExtension) TranslatePrecision(RecognitionPrecision rp)
        {
            return rp switch
            {
                RecognitionPrecision.High or RecognitionPrecision.Default =>
                    (new PageDimensions(1080, 1920), RenderFlags.RenderAnnotations, ImageExtension.Png),
                RecognitionPrecision.Medium =>
                    (new PageDimensions(720, 1280), RenderFlags.RenderAnnotations, ImageExtension.Jpg),
                RecognitionPrecision.Low =>
                (new PageDimensions(720, 1280), RenderFlags.RenderAnnotations | RenderFlags.OptimizeTextForLcd | RenderFlags.Grayscale, ImageExtension.Bmp),
                _ => (new PageDimensions(1080, 1920), RenderFlags.RenderAnnotations, ImageExtension.Png),
            };
        }

        public string TranslateLanguage(RecognitionLanguage rl)
        {
            return rl switch
            {
                RecognitionLanguage.English or RecognitionLanguage.Default => "eng",
                RecognitionLanguage.French => "fra",
                RecognitionLanguage.Polish => "pol",
                _ => "eng",
            };
        }
    }
}
