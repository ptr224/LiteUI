using LiteUI.Navigation;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace LiteUI.Controls
{
    public class Frame : ContentControl, INotifyPropertyChanged, IDisposable
    {
        // StartupPage

        public static readonly DependencyProperty StartupPageProperty = DependencyProperty.Register(
            nameof(StartupPage),
            typeof(Type),
            typeof(Frame),
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public string Title
            => NavigationService.Current?.Title;

        public ToolbarItemsCollection Toolbar
            => NavigationService.Current?.Toolbar;

        /// <summary>
        /// L'oggetto <see cref="Navigation.NavigationService"/> che la pagina sta usando per supportare la navigazione o <see langword="null"/> se questa non è visualizzata.
        /// </summary>
        public NavigationService NavigationService { get; }

        public Frame()
        {
            // Inizializza NavigationService e comando Back
            NavigationService = new NavigationService(OnLoadPageCallback);
        }

        private void OnLoadPageCallback(Page page)
        {
            Content = page;
            NotifyPropertyChanged(null);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Naviga a StartupPage se dichiarata
            if (StartupPage is not null)
                NavigationService.Navigate(StartupPage);
        }

        public virtual void Dispose()
        {
            NavigationService.Dispose();
        }
    }
}
