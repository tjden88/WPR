using System;
using System.Globalization;
using System.Windows.Data;

namespace WPR.MVVM
{
    /// <summary>Конвертер величин</summary>
    public abstract class ConverterBase : IValueConverter
    {

        /// <summary>Преобразование значения</summary>
        /// <param name="v">Преобразуемое значение</param>
        /// <param name="t">Требуемый тип значения</param>
        /// <param name="p">Параметр преобразования</param>
        /// <param name="c">Сведения о культуре</param>
        /// <returns>Преобразованное значение</returns>

        protected abstract object Convert(object v, Type t, object p, CultureInfo c);

        /// <summary>Обратное преобразование значения</summary>
        /// <param name="v">Значение, для которого требуется выполнить обратное преобразование</param>
        /// <param name="t">Требуемый тип данных значения</param>
        /// <param name="p">Параметр преобразования</param>
        /// <param name="c">Сведения о культуре</param>
        /// <returns>Исходное значение</returns>
        /// <exception cref="NotSupportedException">Генерируется при отсутствии переопределения в классах наследниках</exception>

        protected virtual object ConvertBack( object v, Type t, object p, CultureInfo c) => throw new NotSupportedException("Обратное преобразование не поддерживается");

        object IValueConverter.Convert(object v, Type t, object p, CultureInfo c) => Convert(v, t, p, c);

        object IValueConverter.ConvertBack(object v, Type t, object p, CultureInfo c) => ConvertBack(v, t, p, c);
    }
}
