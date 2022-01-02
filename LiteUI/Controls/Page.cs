using LiteUI.Navigation;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace LiteUI.Controls
{
    /// <summary>
    /// Una pagina di contenuto visualizzabile in una <see cref="NavigationWindow"/>.
    /// </summary>
    public class Page : UserControl
    {
        // Title

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(Page),
            new FrameworkPropertyMetadata(nameof(Page), FrameworkPropertyMetadataOptions.AffectsRender)
        );

        [Bindable(true)]
        [Category(nameof(LiteUI))]
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        // Toolbar

        public static readonly DependencyProperty ToolbarProperty = DependencyProperty.Register(
            nameof(Toolbar),
            typeof(ToolbarItemsCollection),
            typeof(Page),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender)
        );

        [Bindable(true)]
        [Category(nameof(LiteUI))]
        public ToolbarItemsCollection Toolbar
        {
            get => (ToolbarItemsCollection)GetValue(ToolbarProperty);
            set => SetValue(ToolbarProperty, value);
        }

        //

        /// <summary>
        /// L'oggetto <see cref="Navigation.NavigationService"/> che la pagina sta usando per supportare la navigazione o <see langword="null"/> se questa non è visualizzata.
        /// </summary>
        public NavigationService NavigationService { get; internal set; }

        /// <summary>
        /// Evento chiamato alla creazione della pagina.
        /// </summary>
        public event EventHandler<NavigationParams> Created;

        /// <summary>
        /// Evento chiamato quando la pagina viene richiamata.
        /// </summary>
        public event EventHandler<NavigationParams> Retrieved;

        /// <summary>
        /// Evento chiamato quando la pagina sta per essere lasciata.
        /// </summary>
        public event CancelEventHandler Leaving;

        /// <summary>
        /// Evento chiamato quando la pagina è stata abbandonata.
        /// </summary>
        public event EventHandler Left;

        internal void CallCreated(NavigationParams extras)
            => Created?.Invoke(this, extras);

        internal void CallRetrieved(NavigationParams extras)
            => Retrieved?.Invoke(this, extras);

        internal bool CallLeaving()
        {
            var e = new CancelEventArgs();
            Leaving?.Invoke(this, e);
            return e.Cancel;
        }

        internal void CallLeft()
            => Left?.Invoke(this, new());
    }
}