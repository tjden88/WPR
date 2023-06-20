using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using WPR.MVVM.Commands.Base;

namespace WPR.Controls.Base;

/// <summary>
/// Базовый шаблон декоратора текстбокса для отображения числовых значений
/// </summary>

[ContentProperty(nameof(TextBox))]
public abstract class NumericDecorator : Control, IDataErrorInfo
{
    static NumericDecorator()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericDecorator), new FrameworkPropertyMetadata(typeof(NumericDecorator)));
    }

    #region PlusMinusButtonsShowing : bool - Показывать кнопки плюс\минус

    /// <summary>Показывать кнопки плюс\минус</summary>
    public static readonly DependencyProperty PlusMinusButtonsShowingProperty =
        DependencyProperty.Register(
            nameof(PlusMinusButtonsShowing),
            typeof(bool),
            typeof(NumericDecorator),
            new PropertyMetadata(false));

    /// <summary>Показывать кнопки плюс\минус</summary>
    [Category("NumericDecorator")]
    [Description("Показывать кнопки плюс, минус")]
    public bool PlusMinusButtonsShowing
    {
        get => (bool)GetValue(PlusMinusButtonsShowingProperty);
        set => SetValue(PlusMinusButtonsShowingProperty, value);
    }

    #endregion


    /// <summary>Текстбокс декоратора</summary>
    public abstract TextBox TextBox { get; set; }

    /// <summary>Увеличить значение</summary>
    public abstract Command IncrementValueCommand { get; }


    /// <summary>Уменьшить значение</summary>
    public abstract Command DecrementValueCommand { get; }

    #region Errors

    /// <summary> Текст ошибки </summary>
    protected string ErrorText;

    public string Error => null;

    public string this[string propertyName] => ErrorText;

    #endregion
}



/// <summary>
/// Типизированный базовый шаблон декоратора текстбокса для отображения числовых значений
/// </summary>

public abstract class NumericDecorator<T> : NumericDecorator where T : struct, IComparable<T>
{

    /// <summary> Происходит при изменении значения </summary>
    public event EventHandler ValueChanged;

    protected NumericDecorator(T defaultIncrement, T defaultMinValue, T defaultMaxValue)
    {
        Increment = defaultIncrement;
        MinValue = defaultMinValue;
        MaxValue = defaultMaxValue;
    }

    #region Props

    #region TextBox : TextBox - Текстбокс декоратора

    /// <summary>Текстбокс декоратора</summary>
    public static readonly DependencyProperty TextBoxProperty =
        DependencyProperty.Register(
            nameof(TextBox),
            typeof(TextBox),
            typeof(NumericDecorator<T>),
            new PropertyMetadata(default(TextBox), (o, e) =>
            {
                ((NumericDecorator<T>)o).SetTextBox((TextBox)e.OldValue, (TextBox)e.NewValue);
            }));

    /// <summary>Текстбокс декоратора</summary>
    [Category("NumericDecorator")]
    [Description("Текстбокс декоратора")]

    [MaybeNull]
    public override TextBox TextBox
    {
        get => (TextBox)GetValue(TextBoxProperty);
        set => SetValue(TextBoxProperty, value);
    }

    #endregion

    #region Value : T - Значение

    /// <summary>Значение</summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(T),
            typeof(NumericDecorator<T>),
            new PropertyMetadata(default(T), (o, e) =>
            {
                ((NumericDecorator<T>)o).OnValueUpdated((T)e.NewValue);
            }, (o, BaseValue) =>
            {
                var numericDecorator = (NumericDecorator<T>)o;
                return numericDecorator.CoerseValue((T)BaseValue, out numericDecorator.ErrorText);
            }));

    /// <summary>Значение</summary>
    [Category("NumericDecorator")]
    [Description("Значение")]
    public T Value
    {
        get => (T)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    #endregion

    #region Increment : T - Шаг изменения значения при использовании кнопок управления или колеса мыши

    /// <summary>Шаг изменения значения при использовании кнопок управления или колеса мыши</summary>
    public static readonly DependencyProperty IncrementProperty =
        DependencyProperty.Register(
            nameof(Increment),
            typeof(T),
            typeof(NumericDecorator<T>),
            new PropertyMetadata(default));

    /// <summary>Шаг изменения значения при использовании кнопок управления или колеса мыши</summary>
    [Category("NumericDecorator")]
    [Description("Шаг изменения значения при использовании кнопок управления или колеса мыши")]
    public T Increment
    {
        get => (T)GetValue(IncrementProperty);
        set => SetValue(IncrementProperty, value);
    }

    #endregion

    #region MinValue : T - Минимальное значение

    /// <summary>Минимальное значение</summary>
    public static readonly DependencyProperty MinValueProperty =
        DependencyProperty.Register(
            nameof(MinValue),
            typeof(T),
            typeof(NumericDecorator<T>),
            new PropertyMetadata(default));

    /// <summary>Минимальное значение</summary>
    [Category("NumericDecorator")]
    [Description("Минимальное значение")]
    public T MinValue
    {
        get => (T)GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }

    #endregion

    #region MaxValue : T - Максимальное значение

    /// <summary>Максимальное значение</summary>
    public static readonly DependencyProperty MaxValueProperty =
        DependencyProperty.Register(
            nameof(MaxValue),
            typeof(T),
            typeof(NumericDecorator<T>),
            new PropertyMetadata(default));

    /// <summary>Максимальное значение</summary>
    [Category("NumericDecorator")]
    [Description("Максимальное значение")]
    public T MaxValue
    {
        get => (T)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    #endregion

    #region AllowTextExpressions : bool - Разрешать ввод текста и символов для расчёта внутри текстбокса

    /// <summary>Разрешать ввод текста и символов для расчёта внутри текстбокса</summary>
    public static readonly DependencyProperty AllowTextExpressionsProperty =
        DependencyProperty.Register(
            nameof(AllowTextExpressions),
            typeof(bool),
            typeof(NumericDecorator<T>),
            new PropertyMetadata(false));

    /// <summary>Разрешать ввод текста и символов для расчёта внутри текстбокса</summary>
    [Category("NumericDecorator")]
    [Description("Разрешать ввод текста и символов для расчёта внутри текстбокса")]
    public bool AllowTextExpressions
    {
        get => (bool)GetValue(AllowTextExpressionsProperty);
        set => SetValue(AllowTextExpressionsProperty, value);
    }

    #endregion

    #region SelectAllOnFocus : bool - При получении фокуса выбрать весь текст

    /// <summary>При получении фокуса выбрать весь текст</summary>
    public static readonly DependencyProperty SelectAllOnFocusProperty =
        DependencyProperty.Register(
            nameof(SelectAllOnFocus),
            typeof(bool),
            typeof(NumericDecorator<T>),
            new PropertyMetadata(default(bool)));

    /// <summary>При получении фокуса выбрать весь текст</summary>
    [Category("NumericDecorator")]
    [Description("При получении фокуса выбрать весь текст")]
    public bool SelectAllOnFocus
    {
        get => (bool)GetValue(SelectAllOnFocusProperty);
        set => SetValue(SelectAllOnFocusProperty, value);
    }

    #endregion

    #region Text : string - Текстовое значение

    /// <summary>Текстовое значение</summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(NumericDecorator<T>),
            new PropertyMetadata(default(string), (o, e) => ((NumericDecorator<T>)o).OnTextChanged((string)e.NewValue)));

    /// <summary>Текстовое значение</summary>
    [Category("NumericDecorator")]
    [Description("Текстовое значение")]
    [MaybeNull]
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    #endregion

    #endregion

    #region Commands

    #region Command IncrementValueCommand - Увеличить значение

    /// <summary>Увеличить значение</summary>
    public override Command IncrementValueCommand => new(OnIncrementValueCommandExecuted, CanIncrementValueCommandExecute, "Увеличить значение");
     

    /// <summary>Проверка возможности выполнения - Увеличить значение</summary>
    private bool CanIncrementValueCommandExecute() => MaxValue.CompareTo(Value) > 0;

    /// <summary>Логика выполнения - Увеличить значение</summary>
    protected void OnIncrementValueCommandExecuted()
    {
        Value = IncrementValue();
        if (TextBox != null) TextBox.SelectionStart = Text?.Length ?? 0;
    }


    #endregion

    #region Command DecrementValueCommand - Уменьшить значение

    /// <summary>Уменьшить значение</summary>
    public override Command DecrementValueCommand => new(OnDecrementValueCommandExecuted, CanDecrementValueCommandExecute, "Уменьшить значение");

    /// <summary>Проверка возможности выполнения - Уменьшить значение</summary>
    private bool CanDecrementValueCommandExecute() => MinValue.CompareTo(Value) < 0;

    /// <summary>Логика выполнения - Уменьшить значение</summary>
    protected void OnDecrementValueCommandExecuted()
    {
        Value = DecrementValue();
        if (TextBox != null) TextBox.SelectionStart = Text?.Length ?? 0;
    }

    #endregion

    #endregion

    #region PropertyChangedHandlers

    private void SetTextBox(TextBox oldValue, TextBox newValue)
    {
        if (oldValue != null)
        {
            oldValue.LostFocus -= TextBox_LostFocus;
            oldValue.GotKeyboardFocus -= TextBoxOnGotKeyboardFocus;
            oldValue.KeyUp -= TextBox_KeyUp;
            oldValue.PreviewTextInput -= TextBox_PreviewTextInput;
            oldValue.MouseWheel -= TextBox_MouseWheel;

            BindingOperations.ClearBinding(oldValue, TextBox.TextProperty);
        }

        if (newValue != null)
        {
            newValue.LostFocus += TextBox_LostFocus;
            newValue.GotKeyboardFocus += TextBoxOnGotKeyboardFocus;
            newValue.KeyUp += TextBox_KeyUp;
            newValue.PreviewTextInput += TextBox_PreviewTextInput;
            newValue.MouseWheel += TextBox_MouseWheel;

            var textBinding = new Binding()
            {
                Path = new PropertyPath(nameof(Text)),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Source = this,
                ValidationRules =
                {
                    new DataErrorValidationRule()
                }
            };
            newValue.SetBinding(TextBox.TextProperty, textBinding);
        }
    }

    private void OnTextChanged(string NewValue)
    {
        const string allowedSymbols = "-.,";

        if (AllowTextExpressions || allowedSymbols.Any(NewValue.EndsWith))
            return;

        CalculateNewValue(false);
    }

    private void OnValueUpdated(T value)
    {
        if (!Equals(value, default(T)))
            Text = SetText(value);
        ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Abstract

    /// <summary> Перевести значение строки в тип значения контрола </summary>
    protected abstract T ParseValue(string TextValue);


    /// <summary> Корректировка значения при установке </summary>
    protected abstract T CoerseValue(T baseValue, out string errorText);


    /// <summary> Установить значение текстбокса при изменении значения </summary>
    protected abstract string SetText(T value);


    /// <summary> Вычислить значение из строки </summary>
    protected abstract T CalculateFromStringExpression(string Expression, out string errorText);


    /// <summary>Увеличить значение</summary>
    protected abstract T IncrementValue();


    /// <summary>Уменьшить значение</summary>
    protected abstract T DecrementValue();

    #endregion

    #region TextBox Events

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        CalculateNewValue();
    }

    private void TextBoxOnGotKeyboardFocus(object Sender, KeyboardFocusChangedEventArgs E)
    {
        if (SelectAllOnFocus)
        {
            TextBox!.Dispatcher.BeginInvoke(new Action(() => TextBox.SelectAll()));
        }
        else
        {
            TextBox!.SelectionStart = TextBox.Text.Length;
        }
    }

    private void TextBox_KeyUp(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Enter:
                CalculateNewValue();
                break;
            case Key.Escape:
                Text = SetText(Value);
                TextBox!.SelectionStart = Text?.Length ?? 0;
                break;
        }
    }

    protected void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (AllowTextExpressions || e.Text.Length < 1) return;

        // берём не выделенный фрагмент текста, т.к выделенный будет удалён
        var checkedText = TextBox!.SelectionLength > 0
            ? Text?.Replace(TextBox.SelectedText, "") ?? string.Empty
            : TextBox.Text;

        //Запрет писать не цифры 
        if (!char.IsDigit(e.Text, 0)) e.Handled = true;

        // Запрет пробелов
        if(e.Text == " ") e.Handled = true;

        // Разрешение писать минус только в начале строки
        if (e.Text == "-" && checkedText.LastIndexOf("-", StringComparison.Ordinal) < 0
                          && TextBox.SelectionStart == 0
                          && MinValue.CompareTo(default) < 0)
        {
            e.Handled = false;
        }

        if (e.Handled)
            e.Handled = DenyTextInput(e.Text, checkedText);
    }

    /// <summary> Дополнительная проверка ввода (true = запретить ввод текущего символа) </summary>
    protected virtual bool DenyTextInput([NotNull] string addedText, [NotNull] string checkedText) => true;


    private void TextBox_MouseWheel(object sender, MouseWheelEventArgs e)
    {

        if (!Equals(Keyboard.FocusedElement, TextBox))
            return;

        if (e.Delta > 0)
            OnIncrementValueCommandExecuted();
        else
            OnDecrementValueCommandExecuted();

        TextBox!.SelectionStart = Text?.Length ?? 0;
        e.Handled = true;
    }


    #endregion

    #region Calculate

    private void CalculateNewValue(bool SetCursorToEnd = true)
    {
        string errorText = null;
        Value = AllowTextExpressions ? CalculateFromStringExpression(Text, out errorText) : ParseValue(Text);

        ErrorText ??= errorText;

        if (!string.IsNullOrWhiteSpace(Text))
            Text = SetText(Value);

        if (TextBox != null && SetCursorToEnd) TextBox.SelectionStart = Text?.Length ?? 0;
    }



    #endregion
}