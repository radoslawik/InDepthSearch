using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace InDepthSearch.UI.Views
{
    public partial class OptionsView : UserControl
    {
        public OptionsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
