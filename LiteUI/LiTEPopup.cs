using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace LiteUI
{
    public enum PopupShadow { All, NoLeft, NoTop, NoRight, NoBottom }

    public class LitePopup : ContentControl
    {
        static LitePopup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LitePopup), new FrameworkPropertyMetadata(typeof(LitePopup)));
        }

        public static readonly DependencyProperty ShadowProperty = DependencyProperty.Register(nameof(Shadow),
            typeof(PopupShadow), typeof(LitePopup), new FrameworkPropertyMetadata(PopupShadow.All, FrameworkPropertyMetadataOptions.AffectsRender));

        [Bindable(true)]
        [Category(nameof(LitePopup))]
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
