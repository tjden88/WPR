﻿using System.Globalization;
using WPR.MVVM.Validation.Base;

namespace WPR.MVVM.Validation;

/// <summary>Строка не может быть пустой</summary>
public class ValidationRuleNotEmptyString : ValidationBase
{
    public ValidationRuleNotEmptyString()
    {
        Message = "Не введено значение";
    }

    protected override bool Validated(object value, CultureInfo cultureInfo)
    {
        return value is string s && !string.IsNullOrWhiteSpace(s);
    }

}