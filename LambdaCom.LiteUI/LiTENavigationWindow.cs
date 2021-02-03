using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace LambdaCom.LiteUI
{
    /// <summary>
    /// Una speciale <see cref="LiTEWindow"/> che contiene e naviga tra delle <see cref="LiTEPage"/>.
    /// </summary>
    public class LiTENavigationWindow : LiTEWindow
    {
        static LiTENavigationWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LiTENavigationWindow), new FrameworkPropertyMetadata(typeof(LiTENavigationWindow)));
        }

        internal Button Back;
        internal Label PageTitle;

        public NavigationService NavigationService { get; }

        public static readonly DependencyProperty HideNavigationButtonProperty = DependencyProperty.Register(nameof(HideNavigationButton),
            typeof(bool), typeof(LiTENavigationWindow), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        [Bindable(true)]
        [Category(nameof(LiTEWindow))]
        public bool HideNavigationButton
        {
            get => (bool)GetValue(HideNavigationButtonProperty);
            set => SetValue(HideNavigationButtonProperty, value);
        }

        public static readonly DependencyProperty StartupPageProperty = DependencyProperty.Register(nameof(StartupPage),
            typeof(Type), typeof(LiTENavigationWindow), new FrameworkPropertyMetadata(null));

        [Bindable(false)]
        [Category(nameof(LiTEWindow))]
        public Type StartupPage
        {
            get => (Type)GetValue(StartupPageProperty);
            set => SetValue(StartupPageProperty, value);
        }

        public LiTENavigationWindow()
        {
            NavigationService = new NavigationService(this);

            Closing += LiTENavigationWindow_Closing;
            Closed += LiTENavigationWindow_Closed;
        }

        private void LiTENavigationWindow_Closing(object sender, CancelEventArgs e)
        {
            // Se la pagina corrente non va chiusa non chiuderti
            e.Cancel = NavigationService.CancelClosing();
        }

        private void LiTENavigationWindow_Closed(object sender, EventArgs e)
        {
            // Disponi le pagine ancora caricate
            NavigationService.Dispose();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Salva riferimenti e gestisci pressione indietro
            Back = (Button)GetTemplateChild("back");
            PageTitle = (Label)GetTemplateChild("title");

            Back.Click += BackButton_Click;

            if (StartupPage != null)
                NavigationService.Navigate(StartupPage);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
