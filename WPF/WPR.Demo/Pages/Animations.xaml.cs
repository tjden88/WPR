using System;
using System.Windows.Controls;
using WPR.Animations;
using WPR.Dialogs;

namespace WPR.Demo.Pages
{
    /// <summary>
    /// Логика взаимодействия для Animations.xaml
    /// </summary>
    public partial class Animations : Page
    {
        public Animations()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new WPRAnimation(Rect)
                .AddDoubleAnimation(1, 0, TimeSpan.FromSeconds(0.3), "Opacity")
                .OnComplete(() => WPRDialogHelper.Bubble(this, "Anim completed"))
                .Begin();
        }
    }
}
