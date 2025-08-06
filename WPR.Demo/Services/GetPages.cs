using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using WPR.Demo.Services.Interfaces;

namespace WPR.Demo.Services
{
    class GetPages : IGetPages
    {
        private const string Nspace = "WPR.Demo.Pages";

        public IEnumerable<Page> GetAllPages()
        {
            var res = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => t.BaseType == typeof(Page) && t.Namespace == Nspace)
                .Select(t => (Page) Activator.CreateInstance(t))
                .OrderBy(t =>
                {
                    var number = int.TryParse($"{t.Tag}", out var tag) ? tag : int.MaxValue;
                    return number;
                })
                .ThenBy(page => page.Title)
                ;

            return res;
        }
    }
}
