using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPR.Extensions
{
    /// <summary>
    /// Анимация кистей текущего стиля
    /// </summary>
    public class BrushStyleDynamicAnimation : AnimationTimeline
    {
        public override Type TargetPropertyType => typeof(Brush);

        public BrushStyleDynamicAnimation()
        {
            Design.BeforeStyleChanged += (_, _) =>
            {
                _From = Design.GetBrushFromResource(FromStyleBrush);
                _To = Design.GetBrushFromResource(ToStyleBrush);
            };
        }

        private SolidColorBrush _From;
        private SolidColorBrush _To;

        private Design.StyleBrush _FromStyleBrush = Design.StyleBrush.None;

        /// <summary> Начальная кисть </summary>
        public Design.StyleBrush FromStyleBrush
        {
            get => _FromStyleBrush;
            set
            {
                _FromStyleBrush = value;
                _From = Design.GetBrushFromResource(value);
            }
        }


        private Design.StyleBrush _ToStyleBrush = Design.StyleBrush.None;

        /// <summary> Конечная кисть </summary>
        public Design.StyleBrush ToStyleBrush
        {
            get => _ToStyleBrush;
            set
            {
                _ToStyleBrush = value;
                _To = Design.GetBrushFromResource(value);
            }
        }

        public override object GetCurrentValue(object defaultOriginValue,
            object defaultDestinationValue,
            AnimationClock animationClock)
        {
            return GetCurrentValue(defaultOriginValue as Brush,
                defaultDestinationValue as Brush,
                animationClock);
        }
        public object GetCurrentValue(Brush defaultOriginValue,
            Brush defaultDestinationValue,
            AnimationClock animationClock)
        {
            if (animationClock?.CurrentProgress == null)
                return Brushes.Transparent;

            //use the standard values if From and To are not set 
            //(it is the value of the given property)
            defaultOriginValue = _From ?? defaultOriginValue;
            defaultDestinationValue = _To ?? defaultDestinationValue;

            return animationClock.CurrentProgress.Value switch
            {
                0 => defaultOriginValue,
                1 => defaultDestinationValue,
                _ => new VisualBrush(new Border()
                {
                    Width = 1,
                    Height = 1,
                    Background = defaultOriginValue,
                    Child = new Border()
                    {
                        Background = defaultDestinationValue,
                        Opacity = animationClock.CurrentProgress.Value,
                    }
                })
            };
        }

        protected override Freezable CreateInstanceCore() => new BrushStyleDynamicAnimation();
    }
}