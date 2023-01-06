using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace WPR.MVVM.ViewModels;

/// <summary>
/// Вьюмодель с возможностью валидации данных при изменении свойств
/// </summary>
public abstract class ValidationViewModel : ViewModel, INotifyDataErrorInfo
{

    /// <summary> Структура ошибки валидации </summary>
    protected readonly struct ValidationRule
    {
        public ValidationRule(string propertyName, Func<bool> rule, string ErrorMessage )
        {
            PropertyName = propertyName;
            Rule = rule;
            this.ErrorMessage = ErrorMessage;
        }

        public readonly string PropertyName;
        public readonly Func<bool> Rule;
        public readonly string ErrorMessage;
    }


    protected ValidationViewModel(bool OnlyForDesignTime = false) : base(OnlyForDesignTime)
    {
    }


    private readonly Dictionary<string, string[]> _ActualErrors = new(); // Текущие ошибки


    /// <summary> Список правил валидации </summary>
    protected abstract List<ValidationRule> ValidationRules { get; }

    protected override bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
    {
        var result = base.Set(ref field, value, PropertyName);

        if (result)
            Validate(value, PropertyName);

        return result;
    }


    protected override void OnPropertyChanged(string PropertyName = null)
    {
        base.OnPropertyChanged(PropertyName);

        if (PropertyName == null) return;
        _ActualErrors.Remove(PropertyName);

        var errors = ValidationRules
            .Where(info => info.PropertyName == PropertyName && !info.Rule.Invoke())
            .Select(info => info.ErrorMessage)
            .ToArray();

        if(errors.Any())
            _ActualErrors[PropertyName] = errors.ToArray();

        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(PropertyName));

        base.OnPropertyChanged(nameof(HasErrors));

    }

    /// <summary> Проверить все правила валидации </summary>
    protected virtual bool CheckHasErrors() => ValidationRules.Any(err => !err.Rule.Invoke());


    public void UpdateErrors()
    {
        var errors = ValidationRules
            .Where(info => !info.Rule.Invoke())
            .ToArray();

        _ActualErrors.Clear();

        foreach (var error in errors)
        {
            _ActualErrors[error.PropertyName] = errors
                .Where(e => e.PropertyName == error.PropertyName)
                .Select(e => e.ErrorMessage)
                .ToArray();
        }

        foreach (var prop in errors.Select(e => e.PropertyName).Distinct())
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(prop));

        base.OnPropertyChanged(nameof(HasErrors));
    }


    private readonly IDictionary<string, List<string>> _Errors = new Dictionary<string, List<string>>();

    private void Validate(object val, string propertyName)
    {
        if (propertyName == null)
            return;

        if (_Errors.ContainsKey(propertyName)) _Errors.Remove(propertyName);

        var context = new ValidationContext(this) { MemberName = propertyName };
        List<ValidationResult> results = new();

        if (!Validator.TryValidateProperty(val, context, results))
        {
            _Errors[propertyName] = results.Select(x => x.ErrorMessage).ToList();
        }

        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    #region IDataError

    public IEnumerable GetErrors(string PropertyName)
    {
        return _Errors.ContainsKey(PropertyName) ? _Errors[PropertyName] : null;
        //if (PropertyName == null) return null;
        //var hasErrors = _ActualErrors.TryGetValue(PropertyName, out var errors);
        //return hasErrors ? errors : null;
    }

    public bool HasErrors => _ActualErrors.Any();

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    #endregion
}