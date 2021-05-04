using System.Windows;
using System.Windows.Media;

namespace System
{
    public static class SystemExtensions
    {
        /// <summary> Поиск визуального родителя по типу </summary>
        /// <typeparam name="T">Тип искомого родительского элемента</typeparam>
        /// <param name="child">Объект, родителя которого надо найти</param>
        public static T FindVisualParent<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            return parentObject is T parent? parent: FindVisualParent<T>(parentObject);

        }

        /// <summary> Поиск логического родителя по типу </summary>
        /// <typeparam name="T">Тип искомого родительского элемента</typeparam>
        /// <param name="child">Объект, родителя которого надо найти</param>
        public static T FindLogicalParent<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = LogicalTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            return parentObject is T parent ? parent : FindLogicalParent<T>(parentObject);

        }

        /// <summary> Поиск визуального потомка по типу </summary>
        /// <typeparam name="T">Тип искомого элемента</typeparam>
        public static T FindVisualChild<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? FindVisualChild<T>(child);
                if (result != null) return result;
            }
            return null;
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

        /// <summary>
        /// Обмен значений 2 переменных
        /// </summary>
        public static void Swap<T>(this ref T first, ref T second) where T: struct
        {
            T tmp = second;
            second = first;
            first = tmp;
        }
    }
}
