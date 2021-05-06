using Avalonia;
using Avalonia.Controls;
using Avalonia.Lottie;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using System;
using System.IO;

namespace InDepthSearch.UI.Views
{
    public class MainWindow : FluentWindow
    {
        private LottieDrawable _lottieDrawable;
        private readonly ContentControl _lottieContainer;
        private readonly Button _searchButton;
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            LottieLog.TraceEnabled = false;
            _lottieContainer = this.FindControl<ContentControl>("lottieControl");
            _searchButton = this.FindControl<Button>("searchButton");
            _searchButton.Click += OnSearchClick;
        }

        private void OnSearchClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            _lottieDrawable.Stop();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            LoadLottie();
        }

        private async void LoadLottie()
        {
            _lottieDrawable = new LottieDrawable();
            _lottieContainer.Content = _lottieDrawable;

            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var json = assets.Open(new Uri("avares://InDepthSearch.UI/Assets/search.json"));
            var str = await new StreamReader(json).ReadToEndAsync();
            var res = await LottieCompositionFactory.FromJsonString(str, "asd");
            if (res is not null) StartLottie(res.Value);
        }

        private void StartLottie(LottieComposition comp)
        {
            _lottieDrawable.SetComposition(comp);
            _lottieDrawable.Start();
            _lottieDrawable.RepeatCount = -1;
            _lottieDrawable.Scale = 0.5f;
        }
    }
}
