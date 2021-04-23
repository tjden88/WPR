using System.Windows;
using System.Windows.Media;

namespace System
{
    public static class SystemExtensions
    {
        /// <summary> Поиск визуального родителя по типу </summary>
        /// <typeparam name="T">Тип искомого родительского элемента</typeparam>
        /// <param name="child">Объект, родителя которого надо найти</param>
        /// <returns></returns>
        public static T FindVisualParent<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            return parentObject is T parent? parent: FindVisualParent<T>(parentObject);

        }

        /// <summary>
        /// Вычислить значение объекта
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static double DoubleValue(this object exp)
        {
            return WPR.Work.Va(exp);
        }

        /// <summary>
        /// Вычислить целое значение объекта
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static int IntValue(this object exp)
        {
            return WPR.Work.VaInt(exp);
        }
    }
}
