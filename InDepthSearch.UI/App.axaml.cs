using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using InDepthSearch.Core.Services;
using InDepthSearch.Core.Types;
using InDepthSearch.Core.ViewModels;
using InDepthSearch.UI.Views;

namespace InDepthSearch.UI
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var window = new MainWindow();
                window.DataContext = new MainViewModel(new OptionService(), new DirectoryService(),
                    new AppService(), new ThemeService(window, Theme.Light));
                desktop.MainWindow = window;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
