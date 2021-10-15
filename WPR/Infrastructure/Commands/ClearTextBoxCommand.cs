using System.Windows.Controls;
using System.Windows.Input;
using WPR.MVVM.Commands;

namespace WPR.Infrastructure.Commands
{
    /// <summary>
    /// Команда очистки поля ввода текстбокса
    /// Параметр - текстбокс
    /// </summary>
    public class ClearTextBoxCommand: BaseCommand
    {
        protected override void Execute(object p)
        {
            if (p is TextBox tbox)
            {
                tbox.Text = string.Empty;
                Keyboard.ClearFocus();
            }
        }

        public override bool CanExecute(object p) => p is TextBox t && !string.IsNullOrEmpty(t.Text);
    }
}
