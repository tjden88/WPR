using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPR.Demo.Services.Interfaces;

namespace WPR.Demo.Services
{
    static class ServiceLocator
    {

        private static IGetPages _PageService;

        /// <summary>Сервис поиска всех страниц</summary>
        public static IGetPages PageService => _PageService ??= new GetPages();

    }
}
