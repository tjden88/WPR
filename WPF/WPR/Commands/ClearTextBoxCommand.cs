using System.Windows.Controls;
using System.Windows.Input;
using WPR.MVVM.Commands.Base;

namespace WPR.Commands;

/// <summary>
/// Команда очистки поля ввода текстбокса
/// Параметр - текстбокс
/// </summary>
public class ClearTextBoxCommand: BaseCommand
{
    /// <summary> Освободить фокус ввода после очистки </summary>
    public bool ClearFocus { get; set; } = true;

    public override void Execute(object p)
    {
        if (p is not TextBox tbox) return;
        tbox.Text = string.Empty;
        if(ClearFocus) Keyboard.ClearFocus();
    }

    public override bool CanExecute(object p) => p is TextBox t && !string.IsNullOrEmpty(t.Text);
}