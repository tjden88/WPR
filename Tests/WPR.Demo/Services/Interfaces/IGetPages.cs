using System.Collections.Generic;
using System.Windows.Controls;

namespace WPR.Demo.Services.Interfaces
{
    interface IGetPages
    {
        public IEnumerable<Page> GetAllPages();
    }
}
