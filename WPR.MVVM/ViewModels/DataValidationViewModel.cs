using System.Collections;
using System.ComponentModel;

namespace WPR.MVVM.ViewModels
{
    /// <summary>
    /// Вьюмодель с возможностью валидации данных при изменении свойств
    /// </summary>
    public abstract class DataValidationViewModel : ViewModel, INotifyDataErrorInfo
    {

        /// <summary> Структура ошибки валидации </summary>
        protected readonly struct ValidationError
        {
            public ValidationError(string propertyName, Func<bool> rule, string Message = "Ошибка")
            {
                PropertyName = propertyName;
                Rule = rule;
                ErrorMessage = Message;
            }

            public readonly string PropertyName;
            public readonly Func<bool> Rule;
            public readonly string ErrorMessage;
        }


        protected DataValidationViewModel(bool OnlyForDesignTime = false) : base(OnlyForDesignTime)
        {
        }


        private readonly Dictionary<string, string[]> _ActualErrors = new(); // Текущие ошибки


        /// <summary> Список правил валидации </summary>
        protected abstract List<ValidationError> ValidationRules { get; }


        protected override void OnPropertyChanged(string PropertyName = null)
        {
            base.OnPropertyChanged(PropertyName);

            if (PropertyName == null) return;
            _ActualErrors.Remove(PropertyName);

            var errors = ValidationRules
                .Where(info => info.PropertyName == PropertyName && info.Rule.Invoke())
                .Select(info => info.ErrorMessage)
                .ToArray();

            if(errors.Any())
                _ActualErrors[PropertyName] = errors.ToArray();

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(PropertyName));

            base.OnPropertyChanged(nameof(HasErrors));
        }

        /// <summary> Проверить все правила валидации </summary>
        protected virtual bool CheckHasErrors() => ValidationRules.Any(err => err.Rule.Invoke());

        public IEnumerable GetErrors(string PropertyName)
        {
            if (PropertyName == null) return null;
            var hasErrors = _ActualErrors.TryGetValue(PropertyName, out var errors);
            return hasErrors ? errors : null;
        }

        public bool HasErrors => _ActualErrors.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public void UpdateErrors()
        {
            var errors = ValidationRules
                .Where(info => info.Rule.Invoke())
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
    }
}
