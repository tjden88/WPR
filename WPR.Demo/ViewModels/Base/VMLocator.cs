using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPR.Demo.ViewModels.Base
{
    internal class VMLocator
    {
        #region MainWindowViewModel : MainWindowViewModel - Модель главного окна

        private MainWindowViewModel _MainWindowViewModel;

        /// <summary>Модель главного окна</summary>
        public MainWindowViewModel MainWindowViewModel
        {
            get => _MainWindowViewModel ??= new MainWindowViewModel();
            set => _MainWindowViewModel = value;
        }

        #endregion
    }
}
