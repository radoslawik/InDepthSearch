using Docnet.Core.Models;
using InDepthSearch.Core.Types;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDepthSearch.Core.Services.Interfaces
{
    public interface IOptionService
    {
        public (PageDimensions, RenderFlags, PixelFormat, ImageFormat) TranslatePrecision(RecognitionPrecision rp);
        public string TranslateLanguage(RecognitionLanguage rl);
    }
}
