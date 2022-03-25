using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using InDepthSearch.Core.Services;
using InDepthSearch.Core.Enums;
using InDepthSearch.Core.ViewModels;
using InDepthSearch.UI.Views;
using Avalonia.Controls;

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
                desktop.MainWindow = new MainWindow
                {
                    DataContext = (Current.FindResource("vmLocator") as ViewModelLocator)!.MainWindow
                };
                desktop.MainWindow.Closed += (s, e) => (Current.FindResource("vmLocator") as ViewModelLocator)!.Dispose();
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}
