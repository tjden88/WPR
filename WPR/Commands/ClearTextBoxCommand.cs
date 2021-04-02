using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPR.Commands
{
    internal class ClearTextBoxCommand: ICommand
    {
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if (parameter is TextBox tbox)
            {
                tbox.Text = "";
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}
