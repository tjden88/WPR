using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPR.MVVM.Commands;

namespace WPR.Controls
{
    /// <summary> Контрол с текстбоксом для ввода числовых данных </summary>
    public class NumericTextBox : Control
    {
        private bool _IsValidNow; // При обновлении Value не обновлять Text, если True

        protected TextBox TextBox { get; set; }

        static NumericTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericTextBox), new FrameworkPropertyMetadata(typeof(NumericTextBox)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TextBox = Template.FindName("PART_TextBox", this) as TextBox;
            if (TextBox == null) throw new ArgumentException(nameof(TextBox));

            TextBox.PreviewTextInput += TextBox_PreviewTextInput;
            TextBox.KeyUp += TextBox_KeyUp;
            TextBox.LostFocus += TextBox_LostFocus;
            TextBox.MouseWheel += TextBox_MouseWheel;
            TextBox.GotKeyboardFocus += TextBoxOnGotKeyboardFocus;
        }

        #region Command PlusButtonCommand - Увеличить значение

        private Command _PlusButtonCommand;

        /// <summary>Увеличить значение</summary>
        public Command PlusButtonCommand => _PlusButtonCommand
            ??= new Command(OnPlusButtonCommandExecuted, CanPlusButtonCommandExecute);

        private bool CanPlusButtonCommandExecute() => Value < MaxValue;

        private void OnPlusButtonCommandExecuted()
        {
            Value = Math.Min(MaxValue, Value + Increment);
        }

        #endregion

        #region Command MinusButtonCommand - Уменьшить значение

        private Command _MinusButtonCommand;

        /// <summary>Уменьшить значение</summary>
        public Command MinusButtonCommand => _MinusButtonCommand
            ??= new Command(OnMinusButtonCommandExecuted, CanMinusButtonCommandExecute);

        private bool CanMinusButtonCommandExecute() => Value > MinValue;

        private void OnMinusButtonCommandExecuted()
        {
            Value = Math.Max(MinValue, Value - Increment);
        }

        #endregion

        #region PlusMinusButtonsShowing : bool - Показывать кнопки плюс\минус

        /// <summary>Показывать кнопки плюс\минус</summary>
        public static readonly DependencyProperty PlusMinusButtonsShowingProperty =
            DependencyProperty.Register(
                nameof(PlusMinusButtonsShowing),
                typeof(bool),
                typeof(NumericTextBox),
                new PropertyMetadata(false));

        /// <summary>Показывать кнопки плюс\минус</summary>
        [Category("Настройки")]
        [Description("Показывать кнопки плюс, минус")]
        public bool PlusMinusButtonsShowing
        {
            get => (bool)GetValue(PlusMinusButtonsShowingProperty);
            set => SetValue(PlusMinusButtonsShowingProperty, value);
        }

        #endregion

        #region Value : double - Значение

        /// <summary>Значение</summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(double),
                typeof(NumericTextBox),
                new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    null, (d, BaseValue) =>
                {
                    var nt = (NumericTextBox)d;

                    if (nt._IsValidNow) return BaseValue;

                    if (nt.Validate(out var validationResult))
                    {
                        var res = Math.Min(nt.MaxValue, Math.Max(nt.MinValue, validationResult));
                        nt.TextExpression = res.ToString(CultureInfo.InvariantCulture);
                    }
                    return default;
                }));

        /// <summary>Значение</summary>
        [Category("Настройки")]
        [Description("Значение")]
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        #endregion

        #region TextExpression : string - Значение текстбокса

        /// <summary>Значение текстбокса</summary>
        internal static readonly DependencyProperty TextExpressionProperty =
            DependencyProperty.Register(
                nameof(TextExpression),
                typeof(string),
                typeof(NumericTextBox),
                new PropertyMetadata(string.Empty, (d, e) =>
                {
                    var nt = (NumericTextBox)d;
                    if (nt.AllowTextExpressions || nt.TextExpression.EndsWith("-") || nt.TextExpression.EndsWith(",") ||
                        nt.TextExpression.EndsWith(".") || !nt.Validate(out var result)) return;

                    nt._IsValidNow = true;
                    nt.Value = result;
                    nt._IsValidNow = false;
                }));

        /// <summary>Значение текстбокса</summary>
        [Category("Настройки")]
        [Description("Значение текстбокса")]
        internal string TextExpression
        {
            get => (string)GetValue(TextExpressionProperty);
            set => SetValue(TextExpressionProperty, value);
        }

        #endregion

        #region DescriptionText : string - Описание ошибок вычислений

        /// <summary>Описание ошибок вычислений</summary>
        internal static readonly DependencyProperty DescriptionTextProperty =
            DependencyProperty.Register(
                nameof(DescriptionText),
                typeof(string),
                typeof(NumericTextBox),
                new PropertyMetadata(default(string)));

        /// <summary>Описание ошибок вычислений</summary>
        //[Category("")]
        [Description("Описание ошибок вычислений")]
        internal string DescriptionText
        {
            get => (string)GetValue(DescriptionTextProperty);
            set => SetValue(DescriptionTextProperty, value);
        }

        #endregion

        #region TextBoxStyle : Style - Стиль для текстбокса

        /// <summary>Стиль для текстбокса</summary>
        public static readonly DependencyProperty TextBoxStyleProperty =
            DependencyProperty.Register(
                nameof(TextBoxStyle),
                typeof(Style),
                typeof(NumericTextBox),
                new PropertyMetadata(default(Style)));

        /// <summary>Стиль для текстбокса</summary>
        [Category("Настройки")]
        [Description("Стиль для текстбокса")]
        public Style TextBoxStyle
        {
            get => (Style)GetValue(TextBoxStyleProperty);
            set => SetValue(TextBoxStyleProperty, value);
        }

        #endregion

        #region Hint : string - Подсказка текстбокса

        /// <summary>Подсказка текстбокса</summary>
        public static readonly DependencyProperty HintProperty =
            DependencyProperty.Register(
                nameof(Hint),
                typeof(string),
                typeof(NumericTextBox),
                new PropertyMetadata(default(string)));

        /// <summary>Подсказка текстбокса</summary>
        [Category("Настройки")]
        [Description("Подсказка текстбокса")]
        public string Hint
        {
            get => (string)GetValue(HintProperty);
            set => SetValue(HintProperty, value);
        }

        #endregion

        #region Increment : double - Шаг изменения значения при использовании кнопок управления или колеса мыши

        /// <summary>Шаг изменения значения при использовании кнопок управления или колеса мыши</summary>
        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register(
                nameof(Increment),
                typeof(double),
                typeof(NumericTextBox),
                new PropertyMetadata(1.0));

        /// <summary>Шаг изменения значения при использовании кнопок управления или колеса мыши</summary>
        [Category("Настройки")]
        [Description("Шаг изменения значения при использовании кнопок управления или колеса мыши")]
        public double Increment
        {
            get => (double)GetValue(IncrementProperty);
            set => SetValue(IncrementProperty, value);
        }

        #endregion

        #region MinValue : double - Минимальное значение

        /// <summary>Минимальное значение</summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(
                nameof(MinValue),
                typeof(double),
                typeof(NumericTextBox),
                new PropertyMetadata(double.MinValue));

        /// <summary>Минимальное значение</summary>
        [Category("Настройки")]
        [Description("Минимальное значение")]
        public double MinValue
        {
            get => (double)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        #endregion

        #region MaxValue : double - Максимальное значение

        /// <summary>Максимальное значение</summary>
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(
                nameof(MaxValue),
                typeof(double),
                typeof(NumericTextBox),
                new PropertyMetadata(double.MaxValue));

        /// <summary>Максимальное значение</summary>
        [Category("Настройки")]
        [Description("Максимальное значение")]
        public double MaxValue
        {
            get => (double)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        #endregion

        #region AllowTextExpressions : bool - Разрешать ввод текста и символов для расчёта внутри текстбокса

        /// <summary>Разрешать ввод текста и символов для расчёта внутри текстбокса</summary>
        public static readonly DependencyProperty AllowTextExpressionsProperty =
            DependencyProperty.Register(
                nameof(AllowTextExpressions),
                typeof(bool),
                typeof(NumericTextBox),
                new PropertyMetadata(false));

        /// <summary>Разрешать ввод текста и символов для расчёта внутри текстбокса</summary>
        [Category("Настройки")]
        [Description("Разрешать ввод текста и символов для расчёта внутри текстбокса")]
        public bool AllowTextExpressions
        {
            get => (bool)GetValue(AllowTextExpressionsProperty);
            set => SetValue(AllowTextExpressionsProperty, value);
        }

        #endregion

        #region DecimalPlaces : int - Количество десятичных знаков

        /// <summary>Количество десятичных знаков</summary>
        public static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.Register(
                nameof(DecimalPlaces),
                typeof(int),
                typeof(NumericTextBox),
                new PropertyMetadata(0));

        /// <summary>Количество десятичных знаков</summary>
        [Category("Настройки")]
        [Description("Количество десятичных знаков")]
        public int DecimalPlaces
        {
            get => (int)GetValue(DecimalPlacesProperty);
            set => SetValue(DecimalPlacesProperty, value);
        }

        #endregion

        //#region IsValid : bool - Валидное ли значение введено

        //[Description("Валидное ли значение введено")]
        //public bool IsValid
        //{
        //    get
        //    {
        //        if (!TextExpression.CalculateStringExpression( out var result, DecimalPlaces))
        //            return false;

        //        return !(result < MinValue) && !(result > MaxValue);
        //    }
        //}

        //#endregion

        #region Calculate

        private void Calculate()
        {
            if (string.IsNullOrWhiteSpace(TextExpression))
            {
                Value = MinValue > 0 ? MinValue : 0.0;
                TextExpression = "";
                DescriptionText = "";
                return;
            }

            if (!TextExpression.CalculateStringExpression(out var result, DecimalPlaces))
            {
                DescriptionText = "Неверное выражение";
                TextExpression = Value.ToString(CultureInfo.InvariantCulture);
                TextBox.SelectionStart = TextBox.Text.Length;
                return;
            }

            DescriptionText = "";
            if (result < MinValue)
            {
                DescriptionText = "Минимальное значение: " + MinValue.ToString(CultureInfo.InvariantCulture);
                result = MinValue;
            }

            if (result > MaxValue)
            {
                DescriptionText = "Максимальное значение: " + MaxValue.ToString(CultureInfo.InvariantCulture);
                result = MaxValue;
            }

            Value = result;
        }

        /// <summary>
        /// Проверить допустимость значения и вывести предупреждение в описании
        /// </summary>
        public bool Validate(out double resultValue)
        {
            var expressionIsValid = !TextBox.Text.CalculateStringExpression(out var result, DecimalPlaces);
            resultValue = result;

            if (expressionIsValid)
            {
                DescriptionText = "Неверное выражение";
                return false;
            }
            DescriptionText = "";
            if (result < MinValue)
            {
                DescriptionText = "Минимальное значение: " + MinValue.ToString(CultureInfo.InvariantCulture);
                return false;
            }

            if (result > MaxValue)
            {
                DescriptionText = "Максимальное значение: " + MaxValue.ToString(CultureInfo.InvariantCulture);
                return false;
            }
            return true;
        }
        #endregion

        #region TextBox Events

        private void TextBox_LostFocus(object sender, RoutedEventArgs e) => Calculate();


        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    Calculate();
                    TextBox.SelectionStart = TextBox.Text.Length;
                    break;
                case Key.Escape:
                    Calculate();
                    Focus();
                    break;
                case Key.Back:
                    DescriptionText = "";
                    break;
            }
        }


        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (AllowTextExpressions) return;

            //Запрет писать не цифры 
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
                return;
            }
            //Десятичные
            if (DecimalPlaces > 0)
            {
                if (e.Text is "." or "," && TextExpression.LastIndexOf(",", StringComparison.Ordinal) < 0
                                         && TextExpression.LastIndexOf(".", StringComparison.Ordinal) < 0
                                         && TextBox.SelectionStart > 0)
                {
                    e.Handled = false;
                }

                // Проверим текущее десятичное после знака
                var currdecimal = Math.Max(TextExpression.LastIndexOf(",", StringComparison.Ordinal),
                    TextExpression.LastIndexOf(".", StringComparison.Ordinal));
                if (currdecimal > -1 && TextBox.SelectionStart - currdecimal > DecimalPlaces)
                {
                    e.Handled = true;
                }
            }
            // Разрешение писать минус только в начале строки
            if (e.Text == "-" && TextExpression.LastIndexOf("-", StringComparison.Ordinal) < 0
                              && TextBox.SelectionStart == 0
                              && MinValue < 0)
            {
                e.Handled = false;
            }
            // Проверим максимум и минимум
            var newval = (TextExpression + e.Text).ConvertToDouble();
            if (newval < MinValue)
            {
                DescriptionText = "Минимальное значение: " + MinValue.ToString(CultureInfo.InvariantCulture);
                e.Handled = true;
            }
            else if (newval > MaxValue)
            {
                DescriptionText = "Максимальное значение: " + MaxValue.ToString(CultureInfo.InvariantCulture);
                e.Handled = true;
            }
            else
            {
                DescriptionText = "";
            }
        }


        private void TextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                PlusButtonCommand.Execute();
            else
                MinusButtonCommand.Execute();
        }


        private void TextBoxOnGotKeyboardFocus(object Sender, KeyboardFocusChangedEventArgs E) =>
            TextBox.Dispatcher.BeginInvoke(new Action(() => TextBox.SelectAll()));

        #endregion
    }
}
