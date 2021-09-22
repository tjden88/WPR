using System;
using System.Collections.Generic;
using System.Linq;

namespace WPR.MVVM
{
    public static class DependensyInjection
    {
        private static Dictionary<Type, (bool IsSingleton, object Instance)> _Services = new();

        public static void AddSingleton<TService>() where TService : class => AddService(typeof(TService), true);

        public static void AddTransient<TService>() where TService : class => AddService(typeof(TService), false);

        private static void AddService(Type t, bool Singleton)
        {
            if (_Services.ContainsKey(t)) throw new ArgumentException($"Сервис {t.Name} уже добавлен в контейнер");
            _Services.Add(t, (Singleton, null));
        }

        public static TService Get<TService>() where TService : class => (TService)GetByType(typeof(TService));

        private static object GetByType(Type t)
        {
            if (!_Services.ContainsKey(t)) throw new NullReferenceException($"Сервис {t.Name} не был добавлен в контейнер");

            if (_Services[t].IsSingleton)
            {
                if (_Services[t].Instance is null)
                {
                    _Services[t] = new(true, CreateInstance(t));
                }
                return _Services[t].Instance;
            }

            return CreateInstance(t);
        }
        private static object CreateInstance(Type t)
        {
            var ctor = t.GetConstructors()
                .First();

            var ctorParameters = ctor
                .GetParameters()
                .Select(p => GetByType(p.ParameterType))
                .ToArray();

            return ctor.Invoke(ctorParameters);
        }
    }
}
