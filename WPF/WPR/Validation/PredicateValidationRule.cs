using System.Globalization;
using WPR.Validation.Base;

namespace WPR.Validation;

/// <summary>Правило валидации на основе проверки предиката</summary>
public class PredicateValidationRule<T> : ValidationBase<T>
{
    public Predicate<T> Predicate { get; set; }

    public PredicateValidationRule(Predicate<T> Predicate) => this.Predicate = Predicate;

    public PredicateValidationRule(Predicate<T> Predicate, string ErrorMessage) : this(Predicate) => Message = ErrorMessage;

    protected override bool Validated(T value, CultureInfo cultureInfo)
    {
       return  Predicate?.Invoke(value) == true;
    }
}