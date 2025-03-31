using System.Windows.Controls;
using System.Windows.Input;

namespace WPR.Commands;

/// <summary>
/// Команда очистки поля ввода текстбокса
/// Параметр - текстбокс
/// </summary>
public class ClearTextBoxCommand: ICommand
{
    /// <summary> Освободить фокус ввода после очистки </summary>
    public bool ClearFocus { get; set; } = true;

    public void Execute(object p)
    {
        if (p is not TextBox tbox) return;
        tbox.Text = string.Empty;
        if(ClearFocus) Keyboard.ClearFocus();
    }

    public event EventHandler CanExecuteChanged;


    public bool CanExecute(object p) => p is TextBox t && !string.IsNullOrEmpty(t.Text);
}