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
        public ValidationRule(string propertyName, Func<bool> rule, string ErrorMessage)
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


    private readonly Dictionary<string, List<string>> _Errors = new(); // Текущие ошибки


    /// <summary> Список правил валидации </summary>
    protected List<ValidationRule> ValidationRules { get; } = new();



    protected override bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
    {
        var result = base.Set(ref field, value, PropertyName);

        if (result)
        {
            Validate(value, PropertyName);
            base.OnPropertyChanged(nameof(HasErrors));
        }

        return result;
    }

    protected override bool SetRef<T>(ref T field, ref T value, [CallerMemberName] string PropertyName = null)
    {
        var result = base.SetRef(ref field, ref value, PropertyName);

        if (result)
        {
            Validate(value, PropertyName);
            base.OnPropertyChanged(nameof(HasErrors));
        }

        return result;
    }


    /// <summary> Проверить все свойства модели </summary>
    public void ValidateAll()
    {
        _Errors.Clear();

        // Проверить правила
        var errors = ValidationRules
            .Where(info => !info.Rule.Invoke())
            .ToArray();

        foreach (var error in errors)
        {
            _Errors[error.PropertyName] = errors
                .Where(e => e.PropertyName == error.PropertyName)
                .Select(e => e.ErrorMessage)
                .ToList();
        }

        // Проверить атрибуты
        var context = new ValidationContext(this);
        var results = new List<ValidationResult>();
        var validateObject = Validator.TryValidateObject(this, context, results, true);

        if (!validateObject)
            foreach (var validationResult in results)
                foreach (var member in validationResult.MemberNames)
                    if (_Errors.ContainsKey(member))
                        _Errors[member].Add(validationResult.ErrorMessage);
                    else
                        _Errors[member] = new List<string>
                        {
                            validationResult.ErrorMessage
                        };


        foreach (var prop in _Errors.Keys)
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(prop));

        base.OnPropertyChanged(nameof(HasErrors));
    }


    // Проверить ошибки изменённого свойства
    private void Validate(object val, string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName)) return;

        _Errors.Remove(propertyName);

        var errorsMessages = new List<string>();

        // Проверить атрибуты
        var context = new ValidationContext(this) { MemberName = propertyName };
        var results = new List<ValidationResult>();
        var validateProperty = Validator.TryValidateProperty(val, context, results);

        if (!validateProperty)
            errorsMessages.AddRange(results.Select(x => x.ErrorMessage));

        // Проверить правила
        var errors = ValidationRules
            .Where(info => info.PropertyName == propertyName && !info.Rule.Invoke())
            .Select(info => info.ErrorMessage);

        errorsMessages.AddRange(errors);

        // Добавить все ошибки в словарь
        if (errorsMessages.Any())
            _Errors[propertyName] = errorsMessages;

        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    #region IDataError

    public IEnumerable GetErrors(string PropertyName)
    {
        if (string.IsNullOrEmpty(PropertyName)) return null;
        return _Errors.ContainsKey(PropertyName) ? _Errors[PropertyName] : null;
    }

    public bool HasErrors => _Errors.Any();

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    #endregion
}