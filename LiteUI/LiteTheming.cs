using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace LiteUI
{
    public static class LiteTheming
    {
        public static readonly DependencyProperty AccentedProperty = DependencyProperty.RegisterAttached(
            "Accented",
            typeof(bool),
            typeof(LiteTheming),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender)
        );

        [AttachedPropertyBrowsableForType(typeof(Button))]
        [Category("LiteTheming")]
        public static bool GetAccented(Button target)
            => (bool)target.GetValue(AccentedProperty);

        public static void SetAccented(Button target, bool value)
            => target.SetValue(AccentedProperty, value);
    }
}
