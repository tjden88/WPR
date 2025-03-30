using System.Windows;
using System.Windows.Media.Animation;
using WPR.Animations;

namespace WPR.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void PagesFrame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if(!PagesFrame.IsLoaded)return;
            new Storyboard()
                .AddDoubleAnimation("RenderTransform.Y", 30, 0, 250, EasingFunctions.ExponentialEaseOut)
                .AddDoubleAnimation("Opacity")
                .ClearOnComplete()
                .Start(PagesFrame);
        }
    }
}
