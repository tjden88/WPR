using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using WPR.Validation;

namespace WPR.Dialogs;

public class InputDialog : DialogBase
{
    public bool MultiLine { get; }

    public int MinTextBoxHeight => MultiLine ? 80 : 20;

    static InputDialog()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(InputDialog), new FrameworkPropertyMetadata(typeof(InputDialog)));
    }
    public InputDialog() : this(false) { }

    public InputDialog(bool MultiLine) : this(null, MultiLine) { }

    public InputDialog(string DefaultValue, bool MultiLine) : this([], DefaultValue, MultiLine) { }

    public InputDialog(IEnumerable<PredicateValidationRule<string>> TextValidationRules, string DefaultValue, bool MultiLine)
    {
        this.MultiLine = MultiLine;
        ViewModel = new ValidationView(TextValidationRules)
        {
            Text = DefaultValue
        };
        ViewModel.CheckErrors();
    }

    protected override bool CanSetCommandExecuted() => ViewModel?.HasErrors == false;

    #region ViewModel : ValidationView - Вьюмодель валидации

    /// <summary>Вьюмодель валидации</summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(
            nameof(ViewModel),
            typeof(ValidationView),
            typeof(InputDialog),
            new PropertyMetadata(default(ValidationView)));

    /// <summary>Вьюмодель валидации</summary>
    [Category("InputDialog")]
    [Description("Вьюмодель валидации")]
    public ValidationView ViewModel
    {
        get => (ValidationView)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    #endregion


    #region Caption : string - Описание

    /// <summary>Описание</summary>
    public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register(
            nameof(Caption),
            typeof(string),
            typeof(InputDialog),
            new PropertyMetadata(default(string)));

    /// <summary>Описание</summary>
    [Category("InputDialog")]
    [Description("Описание")]
    public string Caption
    {
        get => (string)GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }

    #endregion  


    /// <summary> Результат ввода пользователя </summary>
    public string TextValue => ViewModel.Text;



    public class ValidationView(IEnumerable<PredicateValidationRule<string>> TextValidationRules)
        : INotifyDataErrorInfo, INotifyPropertyChanged
    {
        private readonly List<PredicateValidationRule<string>> _TextValidationRules = [.. TextValidationRules];


        #region Text : string - Текст

        /// <summary>Текст</summary>
        private string _Text;

        /// <summary>Текст</summary>
        public string Text
        {
            get => _Text;
            set
            {
                if (Equals(_Text, value)) return;
                _Text = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
                CheckErrors();
            }
        }

        #endregion

        public void CheckErrors()
        {
            _Errors.Clear();
            var text = Text;
            var errors = _TextValidationRules.Where(Rule => !Rule.Validated(text, CultureInfo.InvariantCulture)).Select(Rule => Rule.Message);
            _Errors.AddRange(errors);
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Text)));
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName != nameof(Text)) return null!;
            return _Errors;
        }


        private readonly List<string> _Errors = new();
        public bool HasErrors => _Errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}