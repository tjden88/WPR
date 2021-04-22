using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
