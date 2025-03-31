using System.Windows.Controls;
using System.Windows.Input;
using WPR.Commands.Base;

namespace WPR.Commands;

/// <summary>
/// Команда очистки выбранного элемента комбобокса
/// Параметр - текстбокс
/// </summary>
internal class ClearComboBoxCommand : ICommand
{

    public bool CanExecute(object p) => p is ComboBox c && (c.SelectedIndex > -1 || !string.IsNullOrEmpty(c.Text));

    public void Execute(object p)
    {
        if (p is not ComboBox cbox) return;
        cbox.Text = null!;
        cbox.SelectedValue = null;
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}