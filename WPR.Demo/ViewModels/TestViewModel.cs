using System.Collections.ObjectModel;
using System.IO;
using WPR.Infrastructure.Icons;
using WPR.MVVM.ViewModels;

namespace WPR.Demo.ViewModels
{
    class TestViewModel: ViewModel
    {
        #region Age : int - Тест привязок

        private int _Age;

        /// <summary>Тест привязок</summary>
        public int Age
        {
            get => _Age;
            set => Set(ref _Age, value);
        }

        #endregion

        #region RootDirectory : string - Корневые директории

        private ObservableCollection<DirectoryViewModel> _RootDirectories = new();// = new List<DirectoryViewModel>() {new(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))};

        /// <summary>Корневые директории</summary>
        public ObservableCollection<DirectoryViewModel> RootDirectories
        {
            get => _RootDirectories;
            set => Set(ref _RootDirectories, value);
        }

        #endregion

        #region RootIcon : PackIconKind - Иконка корневой папки

        private PackIconKind _RootIcon = PackIconKind.Drive;

        /// <summary>Иконка корневой папки</summary>
        public PackIconKind RootIcon
        {
            get => _RootIcon;
            set => Set(ref _RootIcon, value);
        }

        #endregion

        #region SelectedDirectory : DirectoryViewModel - Выбранная папка

        private DirectoryViewModel _SelectedDirectory;

        /// <summary>Выбранная папка</summary>
        public DirectoryViewModel SelectedDirectory
        {
            get => _SelectedDirectory;
            set => Set(ref _SelectedDirectory, value);
        }

        #endregion

        public TestViewModel()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Fixed)
                {
                    RootDirectories.Add(new DirectoryViewModel(drive.RootDirectory.FullName));
                }
            }
        }
    }
}
