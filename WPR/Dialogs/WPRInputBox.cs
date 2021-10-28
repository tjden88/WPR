using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WPR.MVVM.Validation;

namespace WPR.Dialogs
{
    public class WPRInputBox : DialogBase
    {
        private readonly IEnumerable<PredicateValidationRule<string>> _TextValidationRules;

        static WPRInputBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WPRInputBox), new FrameworkPropertyMetadata(typeof(WPRInputBox)));
        }

        public WPRInputBox() => _TextValidationRules = Array.Empty<PredicateValidationRule<string>>();

        public WPRInputBox(IEnumerable<PredicateValidationRule<string>> TextValidationRules) => _TextValidationRules = TextValidationRules;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Template.FindName("TextBox", this) is TextBox t)
            {
                Binding binding = BindingOperations.GetBinding(t, TextBox.TextProperty);
                if (binding != null)
                {
                    binding.ValidationRules.Clear();
                    foreach (var rule in _TextValidationRules)
                        binding.ValidationRules.Add(rule);
                    t.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                    t.Focus();
                    t.SelectAll();
                }
            }
        }
        protected override bool CanSetCommandExecuted() => _TextValidationRules.All(Rule => Rule.IsValid);


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
            set => SetValue(TextValueProperty, value);
        }

        #endregion

    }
}
