using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace LiteUI.Controls
{
    public enum PopupShadow { All, NoLeft, NoTop, NoRight, NoBottom }

    public class Popup : ContentControl
    {
        static Popup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Popup), new FrameworkPropertyMetadata(typeof(Popup)));
        }

        // Shadow

        public static readonly DependencyProperty ShadowProperty = DependencyProperty.Register(
            nameof(Shadow),
            typeof(PopupShadow),
            typeof(Popup),
            new FrameworkPropertyMetadata(PopupShadow.All, FrameworkPropertyMetadataOptions.AffectsRender)
        );

        [Bindable(true)]
        [Category(nameof(LiteUI))]
        public PopupShadow Shadow
        {
            get => (PopupShadow)GetValue(ShadowProperty);
            set => SetValue(ShadowProperty, value);
        }
    }
}