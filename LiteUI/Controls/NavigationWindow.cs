using LiteUI.Navigation;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LiteUI.Controls
{
    /// <summary>
    /// Una speciale <see cref="Window"/> che contiene e naviga tra delle <see cref="Page"/>.
    /// </summary>
    public class NavigationWindow : Window
    {
        private class BackRelayCommand : ICommand
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

        static NavigationWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationWindow), new FrameworkPropertyMetadata(typeof(NavigationWindow)));
        }

        // HideNavigationButton

        public static readonly DependencyProperty HideNavigationButtonProperty = DependencyProperty.Register(
            nameof(HideNavigationButton),
            typeof(bool),
            typeof(NavigationWindow),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender)
        );

        [Bindable(true)]
        [Category(nameof(LiteUI))]
        public bool HideNavigationButton
        {
            get => (bool)GetValue(HideNavigationButtonProperty);
            set => SetValue(HideNavigationButtonProperty, value);
        }

        // StartupPage

        public static readonly DependencyProperty StartupPageProperty = DependencyProperty.Register(
            nameof(StartupPage),
            typeof(Type),
            typeof(NavigationWindow),
            new FrameworkPropertyMetadata(null)
        );

        [Bindable(false)]
        [Category(nameof(LiteUI))]
        public Type StartupPage
        {
            get => (Type)GetValue(StartupPageProperty);
            set => SetValue(StartupPageProperty, value);
        }

        //

        public NavigationService NavigationService { get; }
        public ICommand BackCommand { get; }

        public NavigationWindow()
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
            e.Cancel = NavigationService.CancelLeaving();
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
            if (StartupPage is not null)
                NavigationService.Navigate(StartupPage);
        }
    }
}