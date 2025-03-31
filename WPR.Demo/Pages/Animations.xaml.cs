using System.Windows.Controls;
using System.Windows.Media.Animation;
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
            new Storyboard()
                .AddDoubleAnimation("Opacity", 1,0.2,500)
                .OnComplete(() => WPRDialogHelper.Bubble(this, "Anim completed"))
                .OnComplete(() => Rect.Opacity = 0.5)
                .ClearOnComplete()
                .Begin(Rect);
        }
    }
}
