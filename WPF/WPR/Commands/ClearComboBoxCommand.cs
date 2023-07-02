using System.Windows.Controls;
using WPR.MVVM.Commands.Base;

namespace WPR.Commands;

/// <summary>
/// Команда очистки выбранного элемента комбобокса
/// Параметр - текстбокс
/// </summary>
public class ClearComboBoxCommand : BaseCommand
{
    protected override void ExecuteCommand(object p)
    {
        if (p is not ComboBox cbox) return;
        cbox.Text = null!;
        cbox.SelectedValue = null;
    }

    protected override bool CanExecuteCommand(object p) => p is ComboBox c && (c.SelectedIndex > -1 || !string.IsNullOrEmpty(c.Text));

}