using Docnet.Core;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reactive;
using System.Runtime.InteropServices;
using Tesseract;
using System.Linq;
using System.Collections.ObjectModel;
using InDepthSearch.Core.Models;
using InDepthSearch.Core.Types;
using System.Threading;
using InDepthSearch.Core.Services.Interfaces;
using DocumentFormat.OpenXml.Packaging;

namespace InDepthSearch.Core.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public string Logo => "avares://InDepthSearch.UI/Assets/Images/ids-logo.png";

        [Reactive]
        public ObservableCollection<QueryResult> Results { get; set; }
        [Reactive]
        public SearchOptions Options { get; set; }
        [Reactive]
        public ResultStats Stats { get; set; }
        [Reactive]
        public ObservableCollection<RecognitionPrecision> PrecisionOCR { get; set; }
        [Reactive]
        public ObservableCollection<RecognitionLanguage> LanguageOCR { get; set; }
        public ReactiveCommand<Unit, Unit> ReadPDF { get; }
        public ReactiveCommand<Unit, Unit> GetDirectory { get; }
        public ReactiveCommand<Unit, Unit> ChangeTheme { get; }
        public ReactiveCommand<Unit, Unit> ChangeLanguage { get; }

        [Reactive]
        public string ResultInfo { get; set; }
        public bool KeywordErrorVisible => string.IsNullOrWhiteSpace(Options.Keyword);
        public bool PathErrorVisible => !Directory.Exists(Options.Path);
        public bool FormatsErrorVisible => !(Options.UseDOC || Options.UseDOCX || Options.UseODT || Options.UsePDF);
        public bool CanExecute => !KeywordErrorVisible && !PathErrorVisible && !FormatsErrorVisible;
        [Reactive]
        public string AppVersion { get; set; }
        [Reactive]
        public string CurrentThemeName { get; set; }
        [Reactive]
        public string CurrentLanguageName { get; set; }
        [Reactive]
        public bool ItemsReady { get; set; }
        [Reactive]
        public string StatusName { get; set; }


        private Thread? _th;
        private readonly IDocLib _docLib;
        private readonly IOptionService _optionService;
        private readonly IDirectoryService _directoryService;
        private readonly IThemeService _themeService;
        private readonly IAppService _infoService;

        #region Empty constructor only for the designer
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MainViewModel()
        {
            // Initialize variables
            PrecisionOCR = new ObservableCollection<RecognitionPrecision>(Enum.GetValues(typeof(RecognitionPrecision)).Cast<RecognitionPrecision>());
            LanguageOCR = new ObservableCollection<RecognitionLanguage>(Enum.GetValues(typeof(RecognitionLanguage)).Cast<RecognitionLanguage>());
            Options = new SearchOptions("", "", PrecisionOCR.FirstOrDefault(), LanguageOCR.FirstOrDefault(),
                false, true, false, true, false, false, false);
            Results = new ObservableCollection<QueryResult>();
            Stats = new ResultStats("0/0", 0, "0");
            StatusName = SearchStatus.Ready.ToString();
            ResultInfo = "Click search button to start";
            CurrentThemeName = Theme.Default.ToString().ToUpper();
            CurrentLanguageName = AppLanguage.English.ToString().ToUpper();
            ItemsReady = false;
            this.WhenAnyValue(x => x.Options.Keyword, x => x.Options.Path).Subscribe(x => ValidateSelection());

            AppVersion = "x.x.x";
        }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        #endregion
        public MainViewModel(IOptionService optionService, IDirectoryService directoryService, 
            IAppService infoService, IThemeService themeService)
        {
            // Initialize services
            _docLib = DocLib.Instance;
            _optionService = optionService;
            _directoryService = directoryService;
            _themeService = themeService;
            _infoService = infoService;

            // Initialize commands
            GetDirectory = ReactiveCommand.Create(BrowseDirectory);      
            ReadPDF = ReactiveCommand.Create(() => {
                _th = new Thread(() => StartReading());
                _th.IsBackground = true;
                _th.Start();
            });
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

            // Initialize variables
            PrecisionOCR = new ObservableCollection<RecognitionPrecision>(Enum.GetValues(typeof(RecognitionPrecision)).Cast<RecognitionPrecision>());
            LanguageOCR = new ObservableCollection<RecognitionLanguage>(Enum.GetValues(typeof(RecognitionLanguage)).Cast<RecognitionLanguage>());
            Options = new SearchOptions("", "", PrecisionOCR.FirstOrDefault(), LanguageOCR.FirstOrDefault(), 
                false, true, false, true, false, false, false);
            Results = new ObservableCollection<QueryResult>();
            Stats = new ResultStats("", 0, "");
            ResultInfo = infoService.GetSearchInfo(SearchInfo.Init);
            StatusName = infoService.GetSearchStatus(SearchStatus.Ready);
            CurrentThemeName = themeService.GetCurrentThemeName();
            CurrentLanguageName = infoService.GetCurrentLanguage();
            ItemsReady = false;

            // Subscribe to validation values
            this.WhenAnyValue(x => x.Options.Keyword, x => x.Options.Path, x => x.Options.UseDOC, x => x.Options.UseDOCX,
                x => x.Options.UsePDF, x => x.Options.UseODT).Subscribe(x => ValidateSelection());

            // Get assembly version
            AppVersion = infoService.GetVersion();
        }

        private void ValidateSelection()
        {
            this.RaisePropertyChanged(nameof(PathErrorVisible));
            this.RaisePropertyChanged(nameof(KeywordErrorVisible));
            this.RaisePropertyChanged(nameof(FormatsErrorVisible));
            this.RaisePropertyChanged(nameof(CanExecute));
        }

        private void UpdateStringResources()
        {
            CurrentThemeName = _themeService.GetCurrentThemeName();
            StatusName =  _infoService.GetSearchStatus();
            ResultInfo = _infoService.GetSearchInfo();
        }

        void StartReading()
        {
            var searchOptions = Options;

            Results.Clear();
            StatusName = _infoService.GetSearchStatus(SearchStatus.Initializing);
            ItemsReady = false;        
            Stats.FilesAnalyzed = "0/0";
            Stats.PagesAnalyzed = 0;
            Stats.ExecutionTime = "";

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
                ResultInfo = _infoService.GetSearchInfo(SearchInfo.Init);
                Stats.FilesAnalyzed = "0/" + discoveredFiles.Count.ToString();

                foreach (var file in discoveredFiles)
                {
                    System.Diagnostics.Debug.WriteLine("Checking " + file);

                    if (file.EndsWith(".pdf"))
                        HandlePDF(file, searchOptions);
                    else if (file.EndsWith(".docx") || file.EndsWith(".doc"))
                        HandleDOCX(file, searchOptions);
                    
                    fileCounter += 1;
                    Stats.FilesAnalyzed = fileCounter.ToString() + "/" + discoveredFiles.Count.ToString();
                }
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            System.Diagnostics.Debug.WriteLine("Total execution " + elapsedMs);
            Stats.ExecutionTime = (elapsedMs / 1000.0).ToString() + " " + _infoService.GetSecondsString();
            StatusName = _infoService.GetSearchStatus(SearchStatus.Ready);
            if (!Results.Any())
            {
                ResultInfo = _infoService.GetSearchInfo(SearchInfo.NoResults);
                ItemsReady = false;
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
                using var img = _optionService.TranslatePrecision(rp).Item4 == System.Drawing.Imaging.ImageFormat.Tiff ?
                    Pix.LoadTiffFromMemory(imageBytes) : Pix.LoadFromMemory(imageBytes);
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
                if (!ItemsReady) ItemsReady = true;
                at = isCaseSensitive ? text.IndexOf(keyword, searchIndex) : text.ToLower().IndexOf(keyword.ToLower(), searchIndex);
                if (at == -1) break;
                System.Diagnostics.Debug.WriteLine("Found the keyword " + keyword + " in doc: " + filePath + " on page " + pageNum + " at " + at + " position!");
                textBefore = "..." + text.Substring(Math.Max(0, at - offset), at < offset ? at : offset);
                textAfter = text.Substring(at + keyword.Length, at + keyword.Length + offset > text.Length ? text.Length - at - keyword.Length : offset) + "...";
                textFound = text.Substring(at, keyword.Length);
                Results.Add(new QueryResult(MatchConfidence.High, filePath, textBefore, 
                    textFound, textAfter, pageNum));
                searchIndex = at + keyword.Length;
            }
        }

        async void BrowseDirectory()
        {
            var newDir = await _directoryService.ChooseDirectory();
            if (!string.IsNullOrEmpty(newDir))
                Options.Path = newDir;
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
                    using var bmp = new Bitmap(width, height, _optionService.TranslatePrecision(searchOptions.SelectedPrecisionOCR).Item3);

                    AddBytes(bmp, rawBytes);
                    using var stream = new MemoryStream();
                    bmp.Save(stream, _optionService.TranslatePrecision(searchOptions.SelectedPrecisionOCR).Item4);

                    parsedText = ImageToText(stream.ToArray(), searchOptions.SelectedLanguageOCR, searchOptions.SelectedPrecisionOCR);
                }

                SearchPage(parsedText, searchOptions.Keyword, file, i, searchOptions.CaseSensitive);
                Stats.PagesAnalyzed += 1;
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

    }

}
