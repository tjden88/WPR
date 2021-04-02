using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPR.Demo.ViewModels.Base;

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
    }
}
