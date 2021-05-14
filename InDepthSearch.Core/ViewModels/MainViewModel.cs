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
using ReactiveUI.Validation.Helpers;
using ReactiveUI.Validation.Extensions;
using InDepthSearch.Core.Services.Interfaces;

namespace InDepthSearch.Core.ViewModels
{
    public class MainViewModel : ReactiveValidationObject
    {
        public string Logo => "avares://InDepthSearch.UI/Assets/ids-logo.png";

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
        public string FormatsTT => "Choose which formats to search. NOTE: docx and odt are not supported yet.";
        public string SubfolderTT => "Enable if you wish to search in all directories from the selected path.";
        public string CaseSensitiveTT => "Enable if you wish to differentiate between capital and lower-case letters.";
        public string OcrTT => "Enable Optical Character Recognition engine if you wish to search in images.";
        public string LanguageTT => "Specify the language of the keyword. This option could improve the OCR results. Keep default if unsure.";
        public string PrecisionTT => "Choose precision of OCR engine. Setting better precision may cause the search to take more time.";
        [Reactive]
        public string ResultInfo { get; set; }
        [Reactive]
        public bool KeywordErrorVisible { get; set; }
        [Reactive]
        public bool PathErrorVisible { get; set; }
        [Reactive]
        public string AppVersion { get; set; }

        private Thread? _th;
        private readonly IDocLib _docLib;
        private readonly IOptionService _optionService;
        private readonly IDirectoryService _directoryService;

        #region Empty constructor only for the designer
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MainViewModel()
        {
            // Initialize variables
            PrecisionOCR = new ObservableCollection<RecognitionPrecision>(Enum.GetValues(typeof(RecognitionPrecision)).Cast<RecognitionPrecision>());
            LanguageOCR = new ObservableCollection<RecognitionLanguage>(Enum.GetValues(typeof(RecognitionLanguage)).Cast<RecognitionLanguage>());
            Options = new SearchOptions("", "", PrecisionOCR.FirstOrDefault(), LanguageOCR.FirstOrDefault(),
                false, true, false, true, false, false);
            Results = new ObservableCollection<QueryResult>();
            Stats = new ResultStats("Ready", "0/0", true, 0, "0");
            ResultInfo = "Click search button to start";

            // Subscribe for events and set validation rules
            ErrorsChanged += OnValidationErrorsChanged;
            this.ValidationRule(x => x.Options.Keyword, key => !string.IsNullOrEmpty(key), "Keyword cannot be empty!");
            this.ValidationRule(x => x.Options.Path, key => !string.IsNullOrEmpty(key) && Directory.Exists(key), "Path has to be valid!");

            AppVersion = "x.x.x";
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        #endregion
        public MainViewModel(IOptionService optionService, IDirectoryService directoryService, IAppService infoService)
        {
            // Initialize services
            _docLib = DocLib.Instance;
            _optionService = optionService;
            _directoryService = directoryService;

            // Initialize commands
            GetDirectory = ReactiveCommand.Create(BrowseDirectory);      
            ReadPDF = ReactiveCommand.Create(() => {
                _th = new Thread(() => StartReading());
                _th.IsBackground = true;
                _th.Start();
            }, this.IsValid());

            // Initialize variables
            PrecisionOCR = new ObservableCollection<RecognitionPrecision>(Enum.GetValues(typeof(RecognitionPrecision)).Cast<RecognitionPrecision>());
            LanguageOCR = new ObservableCollection<RecognitionLanguage>(Enum.GetValues(typeof(RecognitionLanguage)).Cast<RecognitionLanguage>());
            Options = new SearchOptions("", "", PrecisionOCR.FirstOrDefault(), LanguageOCR.FirstOrDefault(), 
                false, true, false, true, false, false);
            Results = new ObservableCollection<QueryResult>();
            Stats = new ResultStats("Ready", "0/0", true, 0, "0");
            ResultInfo = "Click search button to start";

            // Subscribe for events and set validation rules
            ErrorsChanged += OnValidationErrorsChanged;
            this.ValidationRule(x => x.Options.Keyword, key => !string.IsNullOrEmpty(key), "Keyword cannot be empty!");
            this.ValidationRule(x => x.Options.Path, key => !string.IsNullOrEmpty(key) && Directory.Exists(key), "Path has to be valid!");

            // Get assembly version
            AppVersion = infoService.GetVersion();

        }

        private void OnValidationErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            KeywordErrorVisible = string.IsNullOrWhiteSpace(Options.Keyword);
            PathErrorVisible = !Directory.Exists(Options.Path);
        }

        void StartReading()
        {
            var searchOptions = Options;

            Results.Clear();
            Stats.IsReady = false;
            Stats.Status = "Initializing...";
            Stats.FilesAnalyzed = "0/0";
            Stats.PagesAnalyzed = 0;
            Stats.ExecutionTime = "Waiting to finish";

            var fileCounter = 0;
            var watch = System.Diagnostics.Stopwatch.StartNew();

            if (!string.IsNullOrWhiteSpace(searchOptions.Keyword))
            {
                List<string> discoveredFiles = searchOptions.UseSubfolders ? Directory.GetFiles(searchOptions.Path, "*.pdf", SearchOption.AllDirectories).ToList()
                : Directory.GetFiles(searchOptions.Path, "*.pdf").ToList();

                if (discoveredFiles == null)
                {
                    System.Diagnostics.Debug.WriteLine("No files found... ");
                    return;
                }

                Stats.Status = "Running...";
                Stats.FilesAnalyzed = "0/" + discoveredFiles.Count.ToString();

                foreach (var pdf in discoveredFiles)
                {
                    System.Diagnostics.Debug.WriteLine("Checking " + pdf);
                    using var docReader = _docLib.GetDocReader(pdf, _optionService.TranslatePrecision(searchOptions.SelectedPrecisionOCR).Item1);

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

                        SearchPage(parsedText, searchOptions.Keyword, pdf, i, searchOptions.CaseSensitive);
                        Stats.PagesAnalyzed += 1;

                    }
                    fileCounter += 1;
                    Stats.FilesAnalyzed = fileCounter.ToString() + "/" + discoveredFiles.Count.ToString();
                }
            }
            
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            System.Diagnostics.Debug.WriteLine("Total execution " + elapsedMs);
            Stats.ExecutionTime = (elapsedMs / 1000.0).ToString() + " seconds";
            Stats.IsReady = true;
            Stats.Status = "Ready";
            if (!Results.Any()) ResultInfo = "No results found";

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

    }

}
