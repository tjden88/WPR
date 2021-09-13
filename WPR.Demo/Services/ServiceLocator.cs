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
