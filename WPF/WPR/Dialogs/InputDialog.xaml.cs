using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using WPR.Dialogs.Base;
using WPR.MVVM.Validation;
using WPR.MVVM.ViewModels;

namespace WPR.Dialogs;

public class InputDialog : DialogBase
{

    static InputDialog()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(InputDialog), new FrameworkPropertyMetadata(typeof(InputDialog)));
    }

    public InputDialog() : this(null) { }

    public InputDialog(string DefaultValue) : this(Array.Empty<PredicateValidationRule<string>>(), DefaultValue) { }

    public InputDialog(IEnumerable<PredicateValidationRule<string>> TextValidationRules, string DefaultValue)
    {
        ViewModel = new ValidationView(TextValidationRules)
        {
            Text = DefaultValue
        };
        ViewModel.ValidateAll();
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
        get => (ValidationView) GetValue(ViewModelProperty);
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
        get => (string) GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }

    #endregion  


    /// <summary> Результат ввода пользователя </summary>
    public string TextValue => ViewModel.Text;



    public class ValidationView : ValidationViewModel
    {
        public ValidationView(IEnumerable<PredicateValidationRule<string>> TextValidationRules)
        {
            ValidationRules.AddRange(TextValidationRules
                .Select(tv => new ValidationRule(nameof(Text), () => tv.Predicate.Invoke(Text), tv.Message)));
        }


        #region Text : string - Текст

        /// <summary>Текст</summary>
        private string _Text;

        /// <summary>Текст</summary>
        public string Text
        {
            get => _Text;
            set => Set(ref _Text, value);
        }

        #endregion

        
    }
}