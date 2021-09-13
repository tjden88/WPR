using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WPR.MVVM.ViewModels;

namespace WPR.Demo.ViewModels
{
    /// <summary>Модель представления папки и подпапок файловой системы</summary>
    public class DirectoryViewModel : ViewModel
    {
        private readonly DirectoryInfo _DirectoryInfo;

        /// <summary>Имя папки</summary>
        public string Name => _DirectoryInfo.Name;

        /// <summary>Путь к папке</summary>
        public string Path => _DirectoryInfo.FullName;

        public bool IsRootPath => _DirectoryInfo.Parent is null;

        /// <summary>Подкаталоги</summary>
        public IEnumerable<DirectoryViewModel> SubDidectories
        {
            get
            {
                try
                {
                    return _DirectoryInfo
                        .EnumerateDirectories()
                        .Where(d => (d.Attributes & FileAttributes.System) == 0)
                        .Select(dir => new DirectoryViewModel(dir.FullName));
                }
                catch (UnauthorizedAccessException e)
                {
                    Debug.WriteLine(e.Message);
                }

                return Enumerable.Empty<DirectoryViewModel>();
            }
        }

        public DirectoryViewModel(string Path)
        {
            _DirectoryInfo = new DirectoryInfo(Path);
        }
    }
}
