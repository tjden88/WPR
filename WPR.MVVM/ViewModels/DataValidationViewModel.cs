using System.Collections;
using System.ComponentModel;

namespace WPR.MVVM.ViewModels
{
    /// <summary>
    /// Вьюмодель с возможностью валидации данных при изменении свойств
    /// </summary>
    public class DataValidationViewModel : ViewModel, INotifyDataErrorInfo
    {
        private readonly List<string> _ActualErrors = new();

        /// <summary> Проверка ошибки и описание </summary>
        protected readonly struct Error
        {
            public Error(string propertyName, Func<bool> errorChecker, string Message = "Ошибка")
            {
                PropertyName = propertyName;
                ErrorChecker = errorChecker;
                ErrorMessage = Message;
            }

            public readonly string PropertyName;
            public readonly Func<bool> ErrorChecker;
            public readonly string ErrorMessage;
        }


        /// <summary> Словарь свойств и правил валидации </summary>
        protected List<Error> ErrorInfo { get; } = new();

        protected override void OnPropertyChanged(string PropertyName = null)
        {
            base.OnPropertyChanged(PropertyName);

            _ActualErrors.Clear();
            if (PropertyName == null) return;

            var errors = ErrorInfo
                .Where(info => info.PropertyName == PropertyName)
                .Where(err => err.ErrorChecker.Invoke())
                .Select(err => err.ErrorMessage);
            _ActualErrors.AddRange(errors);

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(PropertyName));
        }

        protected bool CheckAllErrors => ErrorInfo.Any(err => err.ErrorChecker.Invoke());
        
        public IEnumerable GetErrors(string PropertyName) => _ActualErrors;

        public bool HasErrors => _ActualErrors.Count > 0;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    }
}
