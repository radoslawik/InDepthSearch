using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.Styling;
using InDepthSearch.Core.Services.Interfaces;
using InDepthSearch.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDepthSearch.Core.Services
{
    public class ThemeService : IThemeService
    {
        private readonly StyleInclude _lightTheme;
        private readonly StyleInclude _darkTheme;
        private Theme currentTheme;
        private readonly Window _window;

        public ThemeService(Window window, Theme theme)
        {
            _lightTheme = CreateStyle("avares://InDepthSearch.UI/Themes/Light.xaml");
            _darkTheme = CreateStyle("avares://InDepthSearch.UI/Themes/Dark.xaml");
            currentTheme = theme;
            _window = window;
            if (_window.Styles.Count == 0)
                _window.Styles.Add(GetTheme(theme));
            else
                _window.Styles[0] = GetTheme(theme);
        }

        public void ChangeTheme()
        {
            var newTheme = currentTheme == Theme.Light ? Theme.Dark : Theme.Light;
            _window.Styles[0] = GetTheme(newTheme);
            currentTheme = newTheme;
        }

        public StyleInclude GetTheme(Theme t)
        {
            return t switch
            {
                Theme.Light => _lightTheme,
                Theme.Dark => _darkTheme,
                _ => _lightTheme
            };
        }

        private static StyleInclude CreateStyle(string url)
        {
            var self = new Uri("resm:Styles?assembly=InDepthSearch.UI");
            return new StyleInclude(self)
            {
                Source = new Uri(url)
            };
        }

        public string GetCurrentThemeName()
        {
            return currentTheme.ToString().ToUpper();
        }
    }
}
