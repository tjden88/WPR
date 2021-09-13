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


    public class AgeRangeRule : ValidationRule
    {
        public int Min { get; set; }
        public int Max { get; set; }

        public AgeRangeRule()
        {
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int age = 0;

            try
            {
                if (((string)value).Length > 0)
                    age = int.Parse((string)value);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"Illegal characters or {e.Message}");
            }

            if ((age < Min) || (age > Max))
            {
                return new ValidationResult(false,
                    $"Please enter an age in the range: {Min}-{Max}.");
            }
            return ValidationResult.ValidResult;
        }
    }
}
