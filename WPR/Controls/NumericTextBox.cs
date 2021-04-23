using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WPR.Controls
{
    /// <summary>
    /// Контрол с текстбоксом для ввода числовых данных
    /// </summary>
    public class NumericTextBox : Control
    {
        static NumericTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericTextBox), new FrameworkPropertyMetadata(typeof(NumericTextBox)));
        }


        #region PlusMinusButtonsShowing : bool - Показывать кнопки плюс\минус

        /// <summary>Показывать кнопки плюс\минус</summary>
        public static readonly DependencyProperty PlusMinusButtonsShowingProperty =
            DependencyProperty.Register(
                nameof(PlusMinusButtonsShowing),
                typeof(bool),
                typeof(NumericTextBox),
                new PropertyMetadata(default(bool)));

        /// <summary>Показывать кнопки плюс\минус</summary>
        [Category("Настройки")]
        [Description("Показывать кнопки плюс, минус")]
        public bool PlusMinusButtonsShowing
        {
            get => (bool) GetValue(PlusMinusButtonsShowingProperty);
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
                new PropertyMetadata(default(double)));

        /// <summary>Значение</summary>
        [Category("Настройки")]
        [Description("Значение")]
        public double Value
        {
            get => (double) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
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
            get => (Style) GetValue(TextBoxStyleProperty);
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
            get => (string) GetValue(HintProperty);
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
            get => (double) GetValue(IncrementProperty);
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
            get => (double) GetValue(MinValueProperty);
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
            get => (double) GetValue(MaxValueProperty);
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
                new PropertyMetadata(true));

        /// <summary>Разрешать ввод текста и символов для расчёта внутри текстбокса</summary>
        [Category("Настройки")]
        [Description("Разрешать ввод текста и символов для расчёта внутри текстбокса")]
        public bool AllowTextExpressions
        {
            get => (bool) GetValue(AllowTextExpressionsProperty);
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
                new PropertyMetadata(default(int)));

        /// <summary>Количество десятичных знаков</summary>
        [Category("Настройки")]
        [Description("Количество десятичных знаков")]
        public int DecimalPlaces
        {
            get => (int) GetValue(DecimalPlacesProperty);
            set => SetValue(DecimalPlacesProperty, value);
        }

        #endregion

    }
}
