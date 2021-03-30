using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using WPR.Demo.Services.Interfaces;

namespace WPR.Demo.Services
{
    class GetPages : IGetPages
    {
        const string Nspace = "WPR.Demo.Pages";

        public IEnumerable<Page> GetAllPages()
        {
            var res = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => t.BaseType == typeof(Page) && t.Namespace == Nspace)
                .Select(t => (Page) Activator.CreateInstance(t))
                .OrderBy(t => t.Title)
                ;

            return res;
        }
    }
}
