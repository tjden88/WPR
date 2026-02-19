using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WPR.Controls
{
    /// <summary>
    /// Минимальный "дырявый" ScrollViewer: пропускает клики сквозь себя.
    /// </summary>
    public sealed class PassthroughScrollViewer : ScrollViewer
    {
        /// <summary>
        /// Если под курсором нет TreeViewItem (или скроллбара), ScrollViewer становится сквозным.
        /// </summary>
        protected override HitTestResult? HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return null;
        }
    }
}