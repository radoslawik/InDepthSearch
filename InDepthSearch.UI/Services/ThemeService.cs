using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.Styling;
using InDepthSearch.Core.Services.Interfaces;
using InDepthSearch.Core.Enums;
using System;

namespace InDepthSearch.UI.Services
{
    public class ThemeService : IThemeService
    {
        private readonly StyleInclude _lightTheme;
        private readonly StyleInclude _darkTheme;
        private Theme currentTheme = Theme.Light;
        private Window? _window = null;

        public ThemeService()
        {
            _lightTheme = CreateStyle("avares://InDepthSearch.UI/Themes/Light.xaml");
            _darkTheme = CreateStyle("avares://InDepthSearch.UI/Themes/Dark.xaml");
        }

        private void InitDynamicThemes(Theme theme)
        {
            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                _window = desktop.MainWindow;

                if(_window != null)
                {
                    if (_window.Styles.Count == 0)
                        _window.Styles.Add(GetTheme(theme));
                    else
                        _window.Styles[0] = GetTheme(theme);

                    currentTheme = theme;
                }
            }
        }

        public void ChangeTheme()
        {
            var newTheme = currentTheme == Theme.Light ? Theme.Dark : Theme.Light;

            if (_window == null)
            {
                InitDynamicThemes(newTheme);
                return;
            }
            else
            {
                _window.Styles[0] = GetTheme(newTheme);
                currentTheme = newTheme;
            }
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

        private static string GetThemeName(Theme t)
        {
            return t switch
            {
                Theme.Light => (string?)Avalonia.Application.Current.FindResource("ThemeLight") ?? t.ToString().ToUpper(),
                Theme.Dark => (string?)Avalonia.Application.Current.FindResource("ThemeDark") ?? t.ToString().ToUpper(),
                _ => t.ToString().ToUpper()
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
            return GetThemeName(currentTheme);
        }
    }
}
