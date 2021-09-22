using System;
using System.Collections.Generic;
using System.Linq;

namespace WPR.MVVM
{
    public static class DependensyInjection
    {
        private record Service
        {
            public bool IsSingleton { get; init; }
            public Type Implementation { get; init; }
            public object Instance { get; set; }
        }

        private static Dictionary<Type, Service> _Services = new(); // Словарь зарегистрированных типов

        #region Public Methods

        /// <summary> Добавить сервис - одиночку </summary>
        /// <typeparam name="TService">Класс сервиса</typeparam>
        public static void AddSingleton<TService>() where TService : class =>
            AddService(typeof(TService), true);


        ///<summary> Добавить сервис - одиночку с наследником</summary>
        /// <typeparam name="TInterface">Базовый класс сервиса</typeparam>
        /// <typeparam name="TImplementation">Наследник сервиса</typeparam>
        public static void AddSingleton<TInterface, TImplementation>() where TImplementation : TInterface =>
            AddService(typeof(TInterface), true, typeof(TImplementation));


        /// <summary> Добавить сервис - временный (экземпляр будет создан при каждом вызове) </summary>
        /// <typeparam name="TService">Класс сервиса</typeparam>
        public static void AddTransient<TService>() where TService : class =>
            AddService(typeof(TService), false);


        /// <summary> Добавить сервис - временный с наследником (экземпляр будет создан при каждом вызове) </summary>
        /// <typeparam name="TInterface">Базовый класс сервиса</typeparam>
        /// <typeparam name="TImplementation">Наследник сервиса</typeparam>
        public static void AddTransient<TInterface, TImplementation>() where TImplementation : TInterface =>
            AddService(typeof(TInterface), false, typeof(TImplementation));


        /// <summary> Получить экземпляр сервиса </summary>
        /// <typeparam name="TService">Класс сервиса</typeparam>
        /// <param name="parameters">Произвольные параметры конструктора объекта</param>
        /// <returns>Экземпляр класса сервиса или его наследника</returns>
        public static TService Get<TService>(object[] parameters = null) where TService : class =>
            (TService)GetByType(typeof(TService), parameters); 
        #endregion

        #region Private Methods
        private static void AddService(Type t, bool Singleton, Type Implementation = null) // Добавить сервис в словарь
        {
            if (_Services.ContainsKey(t)) throw new ArgumentException($"Сервис {t.Name} уже добавлен в контейнер");

            _Services.Add(t, new Service()
            {
                IsSingleton = Singleton,
                Implementation = Implementation
            });
        }

        private static object GetByType(Type t, object[] parameters) // Получить экземпляр или создать его при необходимости
        {
            if (!_Services.ContainsKey(t)) throw new NullReferenceException($"Сервис {t.Name} не был добавлен в контейнер");

            var requiredService = _Services[t];

            var instanceType = requiredService.Implementation ?? t;

            if (requiredService.IsSingleton)
                return requiredService.Instance ??= CreateInstance(instanceType, parameters);

            return CreateInstance(instanceType, parameters);
        }

        private static object CreateInstance(Type t, object[] parameters) // Создать экземпляр сервиса
        {
            var ctor = t.GetConstructors()
                .First();

            if (parameters != null) return ctor.Invoke(parameters);

            var ctorParameters = ctor
                .GetParameters()
                .Select(p => GetByType(p.ParameterType, null))
                .ToArray();

            return ctor.Invoke(ctorParameters);
        } 
        #endregion
    }
}
