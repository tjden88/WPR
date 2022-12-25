using System.Windows;
using WPR.Dialogs.Base;

namespace WPR.Dialogs
{
 
    public class CustomButtonsDialog : DialogBase
    {
        static CustomButtonsDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomButtonsDialog), new FrameworkPropertyMetadata(typeof(CustomButtonsDialog)));
        }
    }
}
