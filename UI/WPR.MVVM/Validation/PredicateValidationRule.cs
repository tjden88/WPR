using System.Globalization;
using WPR.MVVM.Validation.Base;

namespace WPR.MVVM.Validation;

/// <summary>Правило валидации на основе проверки предиката</summary>
public class PredicateValidationRule<T> : ValidationBase
{
    public Predicate<T> Predicate { get; set; }

    public PredicateValidationRule(Predicate<T> Predicate) => this.Predicate = Predicate;

    public PredicateValidationRule(Predicate<T> Predicate, string ErrorMessage) : this(Predicate) => Message = ErrorMessage;

    protected override bool Validated(object value, CultureInfo cultureInfo) => value is T t && Predicate?.Invoke(t) == true;
}