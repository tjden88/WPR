using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WPR.Controls.Base;
using WPR.Validation;

namespace WPR.Controls
{
    public class WPRInputBox : Dialog
    {
        static WPRInputBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRInputBox), new FrameworkPropertyMetadata(typeof(WPRInputBox)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Template.FindName("TextBox", this) is TextBox t)
            {
                Binding binding = BindingOperations.GetBinding(t, TextBox.TextProperty);
                if (binding != null)
                {
                    binding.ValidationRules.Clear();
                    binding.ValidationRules.Add(_PredicateValidationRule);
                }
            }
        }
        private readonly PredicateValidationRule<string> _PredicateValidationRule = new( S => true);

        /// <summary>Предикат валидации значения. Если возвращает false - в текстовом поле возникает ошибка</summary>
        public Predicate<string> ValidationPredicate { get => _PredicateValidationRule.Predicate; set => _PredicateValidationRule.Predicate = value; }

        /// <summary>Сообщение при ошибке валидации</summary>
        public string ErrorMessage { get => _PredicateValidationRule.Message ; set => _PredicateValidationRule.Message = value; }

        #region TextValue : string - Текстовое значение

        /// <summary>Текстовое значение</summary>
        public static readonly DependencyProperty TextValueProperty =
            DependencyProperty.Register(
                nameof(TextValue),
                typeof(string),
                typeof(WPRInputBox),
                new PropertyMetadata(default(string), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject D, DependencyPropertyChangedEventArgs E)
        {
            WPRInputBox w = (WPRInputBox)D;
            w.TextValue = (string)E.NewValue;
        }

        /// <summary>Текстовое значение</summary>
        //[Category("")]
        [Description("Текстовое значение")]
        public string TextValue
        {
            get => (string) GetValue(TextValueProperty);
            set
            {
                SetValue(TextValueProperty, value);
                if (ValidationPredicate?.Invoke(value) == false)
                {
                    throw new ArgumentException("Неверные данные");
                }
            }
        }

        #endregion

        protected override void OnSetCommandExecute(bool? parameter)
        {



            base.OnSetCommandExecute(parameter);
        }
    }
}
