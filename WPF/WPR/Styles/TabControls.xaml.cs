using System.Windows.Controls;
using System.Windows.Media.Animation;
using WPR.Animations;

namespace WPR.Styles;

partial class TabControls
{
    void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.OriginalSource is not TabControl tc) return;
        if (tc.Template.FindName("PART_SelectedContentHost", tc) is not ContentPresenter cp) return;

         new Storyboard()
            .AddDoubleAnimation("RenderTransform.X", 20, 0, 250, EasingFunctions.SineEaseOut)
            .AddDoubleAnimation("Opacity")
            .ClearOnComplete()
            .Start(cp);
    }
}