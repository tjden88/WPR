using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace WPR.Converters;

/// <summary>
/// Конвертер для отображения значения enum по атрибуту [Description] в XAML.
/// </summary>
public class EnumDescriptionConverter : IValueConverter
{
    /// <summary>
    /// Преобразует enum в строку с учётом атрибута [Description].
    /// Если атрибут отсутствует — возвращает ToString().
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Enum enumValue)
        {
            // Получаем метаданные поля перечисления
            var field = enumValue.GetType().GetField(enumValue.ToString());
            if (field != null)
            {
                // Ищем атрибут [Description]
                var attr = field.GetCustomAttribute<DescriptionAttribute>();
                if (attr != null)
                    return attr.Description;
            }

            // Если атрибута нет — используем ToString()
            return enumValue.ToString();
        }

        return value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Преобразует строку обратно в enum.
    /// Сначала ищет по Description, потом по имени элемента.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType.IsEnum && value is string str)
        {
            foreach (var field in targetType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                // Ищем атрибут [Description]
                var attr = field.GetCustomAttribute<DescriptionAttribute>();
                if ((attr != null && attr.Description == str) || field.Name == str)
                {
                    return Enum.Parse(targetType, field.Name);
                }
            }
        }

        // Если не нашли — ничего не делаем
        return Binding.DoNothing;
    }
}