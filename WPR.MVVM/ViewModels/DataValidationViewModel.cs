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


        protected DataValidationViewModel(bool OnlyForDesignTime = false) : base(OnlyForDesignTime) { }


        private readonly List<string> _ActualErrors = new(); // Текущие ошибки


        /// <summary> Список правил валидации </summary>
        protected abstract List<ValidationError> ValidationRules { get; }


        protected override void OnPropertyChanged(string PropertyName = null)
        {
            base.OnPropertyChanged(PropertyName);

            _ActualErrors.Clear();
            if (PropertyName == null) return;

            var errors = ValidationRules
                .Where(info => info.PropertyName == PropertyName && info.Rule.Invoke())
                .Select(info => info.ErrorMessage);
            _ActualErrors.AddRange(errors);

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(PropertyName));
            base.OnPropertyChanged(nameof(HasErrors));
        }

        /// <summary> Проверить все правила валидации </summary>
        protected virtual bool CheckHasErrors() => ValidationRules.Any(err => err.Rule.Invoke());
        
        public IEnumerable GetErrors(string PropertyName) => _ActualErrors;

        public bool HasErrors => _ActualErrors.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    }
}
