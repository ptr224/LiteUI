using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LambdaCom.LiteUI
{
    /// <summary>
    /// Una speciale <see cref="LiteWindow"/> che contiene e naviga tra delle <see cref="LitePage"/>.
    /// </summary>
    public class LiteNavigationWindow : LiteWindow
    {
        public class BackRelayCommand : ICommand
        {
            private readonly NavigationService _navigationService;

            public BackRelayCommand(NavigationService navigationService)
            {
                _navigationService = navigationService;
            }

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            public bool CanExecute(object parameter)
            {
                return _navigationService.CanGoBack;
            }

            public void Execute(object parameter)
            {
                _navigationService.GoBack();
            }
        }

        static LiteNavigationWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LiteNavigationWindow), new FrameworkPropertyMetadata(typeof(LiteNavigationWindow)));
        }

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

        public NavigationService NavigationService { get; }
        public BackRelayCommand BackCommand { get; }

        public LiteNavigationWindow()
        {
            // Inizializza NavigationService e comando Back
            NavigationService = new NavigationService(page => DataContext = page);
            BackCommand = new BackRelayCommand(NavigationService);

            // Ascolta eventi di chiusura
            Closing += LiteNavigationWindow_Closing;
            Closed += LiteNavigationWindow_Closed;
        }

        private void LiteNavigationWindow_Closing(object sender, CancelEventArgs e)
        {
            // Se la pagina corrente non va chiusa non chiudere nemmeno la finestra
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

            // Naviga a StartupPage se dichiarata
            if (StartupPage != null)
                NavigationService.Navigate(StartupPage);
        }
    }
}
