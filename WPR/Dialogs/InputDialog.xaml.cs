using System.Collections;
using System.ComponentModel;
using System.Windows;

namespace WPR.Dialogs;

public class InputDialog(IEnumerable<InputDialog.ValidationInfo> validation) : DialogBase, INotifyDataErrorInfo
{
    public record ValidationInfo(Predicate<string> Validated, string Message);

    static InputDialog()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(InputDialog),
            new FrameworkPropertyMetadata(typeof(InputDialog)));
    }

    public InputDialog() : this([])
    {
    }

    protected override bool CanSubmit() => !HasErrors;


    #region Props

    public bool MultiLine { get; init; }
    public int MinTextBoxHeight => MultiLine ? 80 : 20;


    

    /// <summary>Текст пользователя</summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(InputDialog),
            new PropertyMetadata(null, OnTextChanged));

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((InputDialog)d).CheckErrors();
    }

    /// <summary>Текст пользователя</summary>
    [Category("InputDialog")]
    [Description("Текст пользователя")]
    public string Text
    {
        get => (string) GetValue(TextProperty);
        init => SetValue(TextProperty, value);
    }

    #endregion

    #region INotifyDataErrorInfo 

    private void CheckErrors()
    {
        _Errors.Clear();
        var text = Text;
        var errors = validation.Where(Rule => !Rule.Validated(text)).Select(Rule => Rule.Message);
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
    #endregion
}

