using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace LambdaCom.LiteUI
{
    /// <summary>
    /// Una speciale <see cref="LiteWindow"/> che contiene e naviga tra delle <see cref="LitePage"/>.
    /// </summary>
    public class LiteNavigationWindow : LiteWindow
    {
        static LiteNavigationWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LiteNavigationWindow), new FrameworkPropertyMetadata(typeof(LiteNavigationWindow)));
        }

        internal Button Back;
        internal Label PageTitle;

        public NavigationService NavigationService { get; }

        public static readonly DependencyProperty HideNavigationButtonProperty = DependencyProperty.Register(nameof(HideNavigationButton),
            typeof(bool), typeof(LiteNavigationWindow), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        [Bindable(true)]
        [Category(nameof(LiteWindow))]
        public bool HideNavigationButton
        {
            get => (bool)GetValue(HideNavigationButtonProperty);
            set => SetValue(HideNavigationButtonProperty, value);
        }

        public static readonly DependencyProperty StartupPageProperty = DependencyProperty.Register(nameof(StartupPage),
            typeof(Type), typeof(LiteNavigationWindow), new FrameworkPropertyMetadata(null));

        [Bindable(false)]
        [Category(nameof(LiteWindow))]
        public Type StartupPage
        {
            get => (Type)GetValue(StartupPageProperty);
            set => SetValue(StartupPageProperty, value);
        }

        public LiteNavigationWindow()
        {
            NavigationService = new NavigationService(this);

            Closing += LiteNavigationWindow_Closing;
            Closed += LiteNavigationWindow_Closed;
        }

        private void LiteNavigationWindow_Closing(object sender, CancelEventArgs e)
        {
            // Se la pagina corrente non va chiusa non chiuderti
            e.Cancel = NavigationService.CancelClosing();
        }

        private void LiteNavigationWindow_Closed(object sender, EventArgs e)
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
