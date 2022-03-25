using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace InDepthSearch.UI.Views
{
    public partial class ResultsView : UserControl
    {
        public ResultsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
