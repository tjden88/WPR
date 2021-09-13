using System;
using System.Globalization;

namespace WPR.MVVM.Validation
{
    /// <summary>Правило валидации на основе проверки предиката</summary>
    public class PredicateValidationRule<T> : ValidationBase
    {
        public Predicate<T> Predicate { get; set; }

        public PredicateValidationRule(Predicate<T> Predicate)
        {
            this.Predicate = Predicate;
        }

        protected override bool Validated(object value, CultureInfo cultureInfo)
        {
            return value is T t && Predicate?.Invoke(t) == true;
        }

    }
}
