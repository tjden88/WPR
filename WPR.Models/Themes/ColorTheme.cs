using System.Drawing;

namespace WPR.Models.Themes
{
    /// <summary>
    /// Описание цветовой темы приложения
    /// </summary>
    public class ColorTheme
    {
        /// <summary> Тип схемы (тёмная, светлая) </summary>
        public ColorThemeTypes ThemeType { get; set; }


        /// <summary> Основной цвет </summary>
        public Color PrimaryColor { get; set; }


        /// <summary> Цвет акцента </summary>
        public Color AccentColor { get; set; }
    }
}
