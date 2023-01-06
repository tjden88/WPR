﻿using System.Windows.Controls;
using System.Windows.Input;
using WPR.MVVM.Commands.Base;

namespace WPR.MVVM.Commands;

/// <summary>
/// Команда очистки поля ввода текстбокса
/// Параметр - текстбокс
/// </summary>
public class ClearTextBoxCommand: BaseCommand
{
    protected override void ExecuteCommand(object p)
    {
        if (p is not TextBox tbox) return;
        tbox.Text = null!;
        Keyboard.ClearFocus();
    }

    protected override bool CanExecuteCommand(object p) => p is TextBox t && !string.IsNullOrEmpty(t.Text);
}