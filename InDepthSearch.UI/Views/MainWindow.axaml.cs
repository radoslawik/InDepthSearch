using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using InDepthSearch.UI.Controls;

namespace InDepthSearch.UI.Views
{
    public class MainWindow : FluentWindow
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
