using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace WPR.Demo.Pages
{
    /// <summary>
    /// Логика взаимодействия для Fields.xaml
    /// </summary>
    public partial class Fields : Page
    {
        public Fields()
        {
            InitializeComponent();
        }

        private void ClearCB_Click(object Sender, RoutedEventArgs E)
        {
          Box.SelectedIndex=-1;
        }
    }

    /// <summary> Пример правила валидации </summary>
    public class AgeRangeRule : ValidationRule
    {
        public int Min { get; set; } = 0;
        public int Max { get; set; } = 99;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is not string val) return new ValidationResult(false, "Неверное значение");

            int age;
            try
            {
                age = int.Parse(val);
            }
            catch (Exception)
            {
                return new ValidationResult(false, $"Неверные символы в значении");
            }

            if ((age < Min) || (age > Max))
            {
                return new ValidationResult(false,
                    $"Введите значение в диапазоне: {Min}-{Max}.");
            }
            return ValidationResult.ValidResult;
        }
    }
}
