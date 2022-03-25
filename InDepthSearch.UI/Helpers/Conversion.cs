using InDepthSearch.Core.Enums;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace InDepthSearch.UI.Helpers
{
    public static class Conversion
    {
        public static ImageFormat ImageExtensionToFormat(ImageExtension ie)
        {
            return ie switch
            {
                ImageExtension.Jpg => ImageFormat.Jpeg,
                ImageExtension.Png => ImageFormat.Png,
                ImageExtension.Bmp => ImageFormat.Bmp,
                _ => ImageFormat.Png,
            };
        }
    }
}
