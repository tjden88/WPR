using System;
using System.Globalization;
using WPR.Validation.Base;

namespace WPR.Validation
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
            if (value is T t)
            {
                if (Predicate?.Invoke(t) == true) return true;
            }
            return false;
        }

    }
}
