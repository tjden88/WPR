using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WPR.MVVM
{
    public static class DependensyInjection
    {
        private static readonly Dictionary<Type, Service> _Services = new(); // Словарь зарегистрированных типов

        private record Service
        {
            public bool IsSingleton { get; init; }
            public Type Implementation { get; init; }
            public object Instance { get; set; }
        }

        /// <summary> Включить логгирование </summary>
        public static bool DebugLogging { get; set; }

        /// <summary> Методы добавления сервисов </summary>
        public class ServiceRegistrator
        {
            /// <summary> Добавить сервис - одиночку </summary>
            /// <typeparam name="TService">Класс сервиса</typeparam>
            public ServiceRegistrator AddSingleton<TService>() where TService : class =>
                AddService(typeof(TService), true);


            ///<summary> Добавить сервис - одиночку с наследником</summary>
            /// <typeparam name="TInterface">Базовый класс сервиса</typeparam>
            /// <typeparam name="TImplementation">Наследник сервиса</typeparam>
            public ServiceRegistrator AddSingleton<TInterface, TImplementation>() where TImplementation : TInterface =>
                AddService(typeof(TInterface), true, typeof(TImplementation));


            /// <summary> Добавить сервис - временный (экземпляр будет создан при каждом вызове) </summary>
            /// <typeparam name="TService">Класс сервиса</typeparam>
            public ServiceRegistrator AddTransient<TService>() where TService : class =>
                AddService(typeof(TService), false);


            /// <summary> Добавить сервис - временный с наследником (экземпляр будет создан при каждом вызове) </summary>
            /// <typeparam name="TInterface">Базовый класс сервиса</typeparam>
            /// <typeparam name="TImplementation">Наследник сервиса</typeparam>
            public ServiceRegistrator AddTransient<TInterface, TImplementation>() where TImplementation : TInterface =>
                AddService(typeof(TInterface), false, typeof(TImplementation));


            private ServiceRegistrator AddService(Type t, bool Singleton, Type Implementation = null) // Добавить сервис в словарь
            {
                if (_Services.ContainsKey(t)) throw new ArgumentException($"Сервис {t.Name} уже добавлен в контейнер");

                _Services.Add(t, new Service()
                {
                    IsSingleton = Singleton,
                    Implementation = Implementation
                });

                return this;
            }
        }

        /// <summary>
        /// Экземпляр класса регистратора для добавления сервисов в контейнер
        /// </summary>
        public static ServiceRegistrator Registrator { get; } = new();

        /// <summary> Получить экземпляр сервиса </summary>
        /// <typeparam name="TService">Класс сервиса</typeparam>
        /// <param name="parameters">Произвольные параметры конструктора объекта</param>
        /// <returns>Экземпляр класса сервиса или его наследника</returns>
        public static TService Get<TService>(params object[] parameters) where TService : class =>
            (TService)GetByType(typeof(TService), parameters);


        public static Task InitializeAsync(CancellationToken cancel = default) => new(() => InitializeSingletonServices(cancel));

        private static void InitializeSingletonServices(CancellationToken cancel)
        {
            foreach (var (type, service) in _Services.Where(service =>
                service.Value.IsSingleton && service.Value.Instance == null))
            {
                if (cancel.IsCancellationRequested)
                    throw new OperationCanceledException("Инициализация сервисов отменена");
                service.Instance = CreateInstance(type, null);
            }
        }

        #region Private Methods

        private static object GetByType(Type t, object[] parameters) // Получить экземпляр или создать его при необходимости
        {
            if (!_Services.ContainsKey(t)) throw new NullReferenceException($"Сервис {t.Name} не был добавлен в контейнер");
            var requiredService = _Services[t];
            var instanceType = requiredService.Implementation ?? t;

            if (requiredService.IsSingleton)
            {
                if (requiredService.Instance == null)
                {
                    Log($"Инициализация Singleton сервиса - {instanceType.Name}");
                    requiredService.Instance = CreateInstance(instanceType, parameters);
                }

                Log($"Выдача Singleton сервиса - {instanceType.Name}");
                return requiredService.Instance;
            }

            Log($"Выдача Transient сервиса - {instanceType.Name}");
            return CreateInstance(instanceType, parameters);
        }


        private static object CreateInstance(Type t, object[] parameters) // Создать экземпляр сервиса
        {
            var ctor = t.GetConstructors().First();

            if (parameters is {Length: > 0}) return ctor.Invoke(parameters);

            var ctorParameters = ctor
                .GetParameters()
                .Select(p => GetByType(p.ParameterType, null))
                .ToArray();

            return ctor.Invoke(ctorParameters);
        }

        private static void Log(string Message)
        {
            if (!DebugLogging) return;
            Debug.WriteLine($"--->>> DependencyInjection Log: {Message}");
        }

        #endregion
    }
}
