using System;

namespace WPR.Tools.Attributes
{
    /// <summary>
    /// Задаёт порядок регистрации/получения сервисов в коллекции <see cref="System.Collections.Generic.IEnumerable{T}"/>.
    /// Меньшее значение — раньше.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ServiceOrderAttribute : Attribute
    {
        /// <summary>
        /// Создаёт атрибут порядка.
        /// </summary>
        /// <param name="order">Порядок. Меньшее значение — раньше.</param>
        public ServiceOrderAttribute(int order)
        {
            Order = order;
        }

        /// <summary>
        /// Порядок регистрации. Меньшее значение — раньше.
        /// </summary>
        public int Order { get; }
    }
}