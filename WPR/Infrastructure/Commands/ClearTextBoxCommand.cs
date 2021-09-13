using System.Windows.Controls;
using System.Windows.Input;
using WPR.MVVM.Commands;

namespace WPR.Infrastructure.Commands
{
    public class ClearTextBoxCommand: BaseCommand
    {
        public override void Execute(object p)
        {
            if (p is TextBox tbox)
            {
                tbox.Text = "";
                Keyboard.ClearFocus();
            }
        }
    }
}
