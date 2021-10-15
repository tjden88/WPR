using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

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
            return parentObject is T parent ? parent : FindVisualParent<T>(parentObject);

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

        private static readonly IFormatProvider _Formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
        private static readonly DataTable _TableForCalculationExpression = new() { Locale = CultureInfo.InvariantCulture }; // Калькулятор

        /// <summary>
        /// Конвертировать объект в double
        /// </summary>
        /// <param name="obj">Объект конвертации</param>
        /// <returns></returns>
        public static double ConvertToDouble(this object obj)
        {
            var parse = obj.ToString()?.Replace(",", ".").Trim();
            double.TryParse(parse, NumberStyles.Any, _Formatter, out var result);
            return result;
        }

        /// <summary>
        /// Конвертировать объект в int
        /// </summary>
        /// <param name="obj">Объект конвертации</param>
        /// <returns></returns>
        public static int ConvertToInt(this object obj)
        {
            var parse = obj.ToString()?.Replace(",", ".").Trim();
            int.TryParse(parse, NumberStyles.Any, _Formatter, out var result);
            return result;
        }

        /// <summary>
        /// Вычислить значение строки
        /// Пустая строка возвращает true
        /// </summary>
        /// <param name="Text">Строка для вычисления</param>
        /// <param name="result">Результат вычисления (при ошибке - 0)</param>
        /// <param name="DecimalPlases">Количество десятичных знаков</param>
        /// <returns>True, если строка имела верный формат и успешно вычислилась</returns>
        public static bool CalculateStringExpression(this string Text, out double result, int DecimalPlases = -1)
        {
            result = default;
            try
            {
                if (string.IsNullOrWhiteSpace(Text)) return true;
                var parse = Text.Replace(",", ".").Trim();
                result = ConvertToDouble(_TableForCalculationExpression.Compute(parse, null));
                if (DecimalPlases > -1)
                {
                    result = Math.Round(result, DecimalPlases);
                }
                return true;
            }
            catch (DataException)
            {
                return false;
            }
        }

        /// <summary>
        /// Потокобезопасное действие через Dispatcher
        /// </summary>
        /// <param name="obj">Диспетчеризуемый объект</param>
        /// <param name="Action">Делегат действия</param>
        public static void DoDispatherAction(this DispatcherObject obj, Action Action)
        {
            if (obj.Dispatcher.CheckAccess())
            {
                Action?.Invoke();
            }
            else
            {
                obj.Dispatcher.Invoke(Action);
            }
        }

        /// <summary>
        /// Потокобезопасное действие через Dispatcher текущего приложения
        /// </summary>
        /// <param name="Action">Делегат действия</param>
        public static void AppDispatherAction(Action Action) => DoDispatherAction(Application.Current, Action);
    }
}
