using System.Windows.Controls;
using System.Windows.Input;
using WPR.MVVM.Commands;

namespace WPR.Commands
{
    internal class ClearTextBoxCommand: BaseCommand
    {
        protected override void Execute(object p)
        {
            if (p is TextBox tbox)
            {
                tbox.Text = "";
                Keyboard.ClearFocus();
            }
        }
    }
}
