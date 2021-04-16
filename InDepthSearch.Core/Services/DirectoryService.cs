using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using InDepthSearch.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InDepthSearch.Core.Services
{
    public class DirectoryService : IDirectoryService
    {
        public async Task<string?> ChooseDirectory()
        {
            OpenFolderDialog dialog = new OpenFolderDialog();
            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                return await dialog.ShowAsync(desktop.MainWindow);
            else
                return null;
        }
    }
}
