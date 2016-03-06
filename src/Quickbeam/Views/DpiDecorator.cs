using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Quickbeam.Views
{
    public class DpiDecorator : Decorator
    {
        public DpiDecorator()
        {
            Loaded += (s, e) =>
            {
                var presentationSource = PresentationSource.FromVisual(this);
                if (presentationSource == null) return;
                if (presentationSource.CompositionTarget == null) return;

                var matrix = presentationSource.CompositionTarget.TransformToDevice;
                var dpiTransform = new ScaleTransform(1 / matrix.M11, 1 / matrix.M22);
                if (dpiTransform.CanFreeze)
                    dpiTransform.Freeze();
                LayoutTransform = dpiTransform;
            };
        }

    }
}
