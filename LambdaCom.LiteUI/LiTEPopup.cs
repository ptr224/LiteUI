using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

namespace LambdaCom.LiteUI
{
    public enum PopupShadow { All, NoLeft, NoTop, NoRight, NoBottom }

    public class LiTEPopup : ContentControl
    {
        static LiTEPopup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LiTEPopup), new FrameworkPropertyMetadata(typeof(LiTEPopup)));
        }

        public static readonly DependencyProperty ShadowProperty = DependencyProperty.Register(nameof(Shadow),
            typeof(PopupShadow), typeof(LiTEPopup), new FrameworkPropertyMetadata(PopupShadow.All, FrameworkPropertyMetadataOptions.AffectsRender));

        [Bindable(true)]
        [Category(nameof(LiTEPopup))]
        public PopupShadow Shadow
        {
            get => (PopupShadow)GetValue(ShadowProperty);
            set => SetValue(ShadowProperty, value);
        }

        /*protected override async void OnOpened(EventArgs e)
        {
            base.OnOpened(e);

            //Attiva l'effetto blur (su thread UI per non rallentare)
            await Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                var hwnd = (HwndSource)PresentationSource.FromVisual(Child);
                AcrylicHelper.EnableBlur(hwnd.Handle, AccentFlagsType.Popup);
            }));
        }*/
    }
}
