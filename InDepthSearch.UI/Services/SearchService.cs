
using Docnet.Core;
using DocumentFormat.OpenXml.Packaging;
using InDepthSearch.Core.Enums;
using InDepthSearch.Core.Managers.Interfaces;
using InDepthSearch.Core.Models;
using InDepthSearch.Core.Services.Interfaces;
using InDepthSearch.UI.Helpers;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Tesseract;

namespace InDepthSearch.UI.Services
{
    public class SearchService : ISearchService
    {
        private readonly IDocLib _docLib;
        private readonly IOptionService _optionService;
        private readonly IResultManager _resultManager;
        public SearchService(IOptionService optionService, IResultManager resultManager)
        {
            _docLib = DocLib.Instance;
            _optionService = optionService;
            _resultManager = resultManager;
        }

        public void Search(string file, SearchOptions searchOptions)
        {
            if(file.EndsWith(".pdf"))
            {
                HandlePDF(file, searchOptions);
            }
            else if (file.EndsWith(".docx") || file.EndsWith(".doc"))
            {
                HandleDOCX(file, searchOptions);
            }
        }

        private void HandlePDF(string file, SearchOptions searchOptions)
        {
            using var docReader = _docLib.GetDocReader(file, _optionService.TranslatePrecision(searchOptions.SelectedPrecisionOCR).Item1);

            for (var i = 0; i < docReader.GetPageCount(); i++)
            {
                using var pageReader = docReader.GetPageReader(i);
                var parsedText = pageReader.GetText().ToString();

                if (searchOptions.UseOCR && string.IsNullOrWhiteSpace(parsedText))
                {
                    var rawBytes = pageReader.GetImage(_optionService.TranslatePrecision(searchOptions.SelectedPrecisionOCR).Item2);
                    var width = pageReader.GetPageWidth();
                    var height = pageReader.GetPageHeight();
                    var imFormat = Conversion.ImageExtensionToFormat(_optionService.TranslatePrecision(searchOptions.SelectedPrecisionOCR).Item3);
                    using var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                    AddBytes(bmp, rawBytes);
                    using var stream = new MemoryStream();
                    bmp.Save(stream, Conversion.ImageExtensionToFormat(_optionService.TranslatePrecision(searchOptions.SelectedPrecisionOCR).Item3));

                    parsedText = ImageToText(stream.ToArray(), searchOptions.SelectedLanguageOCR, searchOptions.SelectedPrecisionOCR);
                }

                SearchPage(parsedText, searchOptions.Keyword, file, i, searchOptions.CaseSensitive);
                _resultManager.Stats.PagesAnalyzed += 1;
            }
        }
        private void HandleDOCX(string file, SearchOptions searchOptions)
        {
            using WordprocessingDocument wordDocument = WordprocessingDocument.Open(file, false);
            var paragraphs = wordDocument.MainDocumentPart?.Document?.Body?.ChildElements;
            var images = wordDocument.MainDocumentPart?.ImageParts;
            if (searchOptions.UseOCR && images != null)
            {
                foreach (var image in images)
                {
                    var docStream = wordDocument.Package.GetPart(image.Uri).GetStream();
                    using var stream = new MemoryStream();
                    docStream.CopyTo(stream);
                    var parsedText = ImageToText(stream.ToArray(), searchOptions.SelectedLanguageOCR, searchOptions.SelectedPrecisionOCR);
                    SearchPage(parsedText, searchOptions.Keyword, file, -1, searchOptions.CaseSensitive);
                }
            }

            if (paragraphs != null)
            {
                var parsedString = "";
                foreach (var paragraph in paragraphs)
                    parsedString = parsedString + paragraph.InnerText + "\r";

                SearchPage(parsedString, searchOptions.Keyword, file, -1, searchOptions.CaseSensitive);
            }
        }

        private static void AddBytes(Bitmap bmp, byte[] rawBytes)
        {
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

            var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);
            var pNative = bmpData.Scan0;

            Marshal.Copy(rawBytes, 0, pNative, rawBytes.Length);
            bmp.UnlockBits(bmpData);
        }

        string ImageToText(byte[] imageBytes, RecognitionLanguage rl, RecognitionPrecision rp)
        {
            try
            {
                using var engine = new TesseractEngine(@"./Files", _optionService.TranslateLanguage(rl), EngineMode.Default);
                using var img = Pix.LoadFromMemory(imageBytes);
                using var pager = engine.Process(img);
                return pager.GetText().ToString();
                //System.Diagnostics.Debug.WriteLine("Mean confidence: {0}", pager.GetMeanConfidence());
                //System.Diagnostics.Debug.WriteLine("Text {0}", text);
            }
            catch (Exception ee)
            {
                System.Diagnostics.Debug.WriteLine("Unexpected Error: " + ee.Message);
                System.Diagnostics.Debug.WriteLine("Details: ");
                System.Diagnostics.Debug.WriteLine(ee.ToString());
            }

            return "";
        }

        void SearchPage(string rawText, string keyword, string filePath, int pageNum, bool isCaseSensitive)
        {

            var searchIndex = 0;
            var at = 0;
            string textBefore, textFound, textAfter;
            var offset = 30;
            var text = rawText.Replace("\n", " ").Replace("\r", " ");

            while (at > -1)
            {
                if (!_resultManager.ItemsReady) _resultManager.SetItemsReady(true);
                at = isCaseSensitive ? text.IndexOf(keyword, searchIndex) : text.ToLower().IndexOf(keyword.ToLower(), searchIndex);
                if (at == -1) break;
                System.Diagnostics.Debug.WriteLine("Found the keyword " + keyword + " in doc: " + filePath + " on page " + pageNum + " at " + at + " position!");
                textBefore = "..." + text.Substring(Math.Max(0, at - offset), at < offset ? at : offset);
                textAfter = text.Substring(at + keyword.Length, at + keyword.Length + offset > text.Length ? text.Length - at - keyword.Length : offset) + "...";
                textFound = text.Substring(at, keyword.Length);
                _resultManager.AddResult(new QueryResult(MatchConfidence.High, filePath, textBefore, textFound, textAfter, pageNum));
                searchIndex = at + keyword.Length;
            }
        }
    }
}
