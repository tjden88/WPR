using System.Globalization;

namespace WPR.MVVM.Validation
{
    /// <summary>Строка не может быть пустой</summary>
    public class ValidationRuleNotEmptyString : ValidationBase
    {
        public ValidationRuleNotEmptyString()
        {
            Message = "Не введено значение";
        }

        protected override bool Validated(object value, CultureInfo cultureInfo)
        {
            return value is string s && s.Trim().Length > 0;
        }

    }
}