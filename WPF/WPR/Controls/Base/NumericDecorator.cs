using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace WPR.Controls.Base;

/// <summary>
/// Базовый шаблон декоратора текстбокса для отображения числовых значений
/// </summary>
/// <typeparam name="T"></typeparam>
[ContentProperty(nameof(TextBox))]
public abstract class NumericDecorator<T> : Control, IDataErrorInfo
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
    public TextBox TextBox
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
                ((NumericDecorator<T>)o).SetStringValue((T)e.NewValue);
            }, (o, BaseValue) =>
            {
                return ((NumericDecorator<T>)o).CoerseValue((T)BaseValue);
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

    #region PlusMinusButtonsShowing : bool - Показывать кнопки плюс\минус

    /// <summary>Показывать кнопки плюс\минус</summary>
    public static readonly DependencyProperty PlusMinusButtonsShowingProperty =
        DependencyProperty.Register(
            nameof(PlusMinusButtonsShowing),
            typeof(bool),
            typeof(NumericDecorator<T>),
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
            new PropertyMetadata(default(string)));

    /// <summary>Текстовое значение</summary>
    [Category("NumericDecorator")]
    [Description("Текстовое значение")]
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    #endregion

    #endregion

    private void SetTextBox(TextBox oldValue, TextBox newValue)
    {
        if (oldValue != null)
        {
            oldValue.LostFocus -= TextBox_LostFocus;
            oldValue.GotKeyboardFocus -= TextBoxOnGotKeyboardFocus;
            BindingOperations.ClearBinding(oldValue, TextBox.TextProperty);
        }

        if (newValue != null)
        {
            newValue.LostFocus += TextBox_LostFocus;
            newValue.GotKeyboardFocus += TextBoxOnGotKeyboardFocus;

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

    private void SetStringValue(T value)
    {
        if (TextBox == null)
            throw new ArgumentNullException(nameof(TextBox));

        TextBox.Text = SetText(value);
    }

    /// <summary> Перевести значение строки в тип значения контрола </summary>
    protected abstract T ParseValue(string TextValue);


    /// <summary> Корректировка значения при установке </summary>
    protected abstract T CoerseValue(T baseValue);


    /// <summary> Установить значение текстбокса при изменении значения </summary>
    protected abstract string SetText(T value);


    #region TextBox Events

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        Value = ParseValue(TextBox!.Text);
        TextBox.Text = SetText(Value);
    }

    private void TextBoxOnGotKeyboardFocus(object Sender, KeyboardFocusChangedEventArgs E)
    {
        if (SelectAllOnFocus) TextBox!.Dispatcher.BeginInvoke(new Action(() => TextBox.SelectAll()));
    }

    #endregion

    public string Error
    {
        get
        {
            Debug.WriteLine("error prop");
            return "Error!";
        }
    }

    public string this[string columnName]
    {
        get
        {
            Debug.WriteLine(columnName);

            return $"Column: {columnName}";
        }
    }

    private readonly List<string> _Errors = new();

    public IEnumerable GetErrors(string propertyName)
    {
        if (!_Errors.Contains(propertyName))
            _Errors.Add(propertyName);

        return _Errors;
    }

    public bool HasErrors => true;

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
}