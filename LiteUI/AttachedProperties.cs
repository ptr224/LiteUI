using System.Windows;
using System.Windows.Controls;

namespace LiteUI
{
    public static class AttachedProperties
    {
        public static readonly DependencyProperty AccentedProperty = DependencyProperty.RegisterAttached(
            "Accented",
            typeof(bool),
            typeof(AttachedProperties),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender)
        );

        public static bool GetAccented(Button target)
            => (bool)target.GetValue(AccentedProperty);

        public static void SetAccented(Button target, bool value)
            => target.SetValue(AccentedProperty, value);
    }
}
