namespace WPR.Domain.Themes
{
    /// <summary>
    /// Описание цветовой темы приложения
    /// </summary>
    public class ColorTheme
    {
        /// <summary> Тип схемы (тёмная, светлая) </summary>
        public ColorThemeTypes ThemeType { get; set; }


        /// <summary> Основной цвет </summary>
        public string PrimaryColor { get; set; } = null!;


        /// <summary> Цвет акцента </summary>
        public string AccentColor { get; set; } = null!;
    }
}
