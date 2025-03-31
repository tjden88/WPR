using System.Windows.Controls;
using System.Windows.Input;

namespace WPR.Commands;

internal class ClearTextBoxCommand : ICommand
{
    /// <summary> Освободить фокус ввода после очистки </summary>
    public bool ClearFocus { get; set; } = true;


    public bool CanExecute(object p) => p is TextBox t && !string.IsNullOrEmpty(t.Text);

    public void Execute(object p)
    {
        if (p is not TextBox tbox) return;
        tbox.Text = string.Empty;
        if (ClearFocus) Keyboard.ClearFocus();
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

}