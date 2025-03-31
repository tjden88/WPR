using System.Windows.Controls;
using System.Windows.Input;

namespace WPR.Commands;

/// <summary>
/// Команда очистки выбранного элемента комбобокса
/// Параметр - текстбокс
/// </summary>
public class ClearComboBoxCommand : ICommand
{
    public void Execute(object p)
    {
        if (p is not ComboBox cbox) return;
        cbox.Text = null!;
        cbox.SelectedValue = null;
    }

    public bool CanExecute(object p) => p is ComboBox c && (c.SelectedIndex>-1 || !string.IsNullOrEmpty(c.Text));

    public event EventHandler CanExecuteChanged;
}