using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace LiteUI
{
    /// <summary>
    /// Descrive le modalità di lancio di una pagina.
    /// </summary>
    public enum PageLaunchMode
    {
        /// <summary>
        /// Crea una nuova pagina che non viene salvata nella cronologia.
        /// </summary>
        Ignore,

        /// <summary>
        /// Crea ogni volta una nuova pagina.
        /// </summary>
        Normal,

        /// <summary>
        /// Se la pagina è attualmente in primo piano viene richiamata la stessa istanza.
        /// </summary>
        SingleTop,

        /// <summary>
        /// Se la pagina è presente nella cronologia richiama la stessa istanza ed elimina tutte le pagine accodate successivamente.
        /// </summary>
        SingleInstance
    }

    /// <summary>
    /// Specifica le opzioni relative alla <see cref="LitePage"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class PageOptionsAttribute : Attribute
    {
        /// <summary>
        /// Specifica le modalità di lancio della pagina.
        /// </summary>
        public PageLaunchMode LaunchMode { get; set; } = PageLaunchMode.Normal;
    }

    /// <summary>
    /// Una pagina di contenuto visualizzabile in una <see cref="LiteNavigationWindow"/>.
    /// </summary>
    public class LitePage : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(LitePage),
            new FrameworkPropertyMetadata(nameof(LitePage), FrameworkPropertyMetadataOptions.AffectsRender)
        );

        [Bindable(true)]
        [Category(nameof(LitePage))]
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty ToolbarProperty = DependencyProperty.Register(
            nameof(Toolbar),
            typeof(ToolbarItemsCollection),
            typeof(LitePage),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender)
        );

        [Bindable(true)]
        [Category(nameof(LitePage))]
        public ToolbarItemsCollection Toolbar
        {
            get => (ToolbarItemsCollection)GetValue(ToolbarProperty);
            set => SetValue(ToolbarProperty, value);
        }

        /// <summary>
        /// L'oggetto <see cref="LiteUI.NavigationService"/> che la pagina sta usando per supportare la navigazione o <see langword="null"/> se questa non è visualizzata.
        /// </summary>
        public NavigationService NavigationService
            => GetWindow()?.NavigationService;

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
        public event CancelEventHandler Closing;

        internal void CallCreated(NavigationParams extras)
            => Created?.Invoke(this, extras);

        internal void CallRetrieved(NavigationParams extras)
            => Retrieved?.Invoke(this, extras);

        internal bool CallClosing()
        {
            var e = new CancelEventArgs();
            Closing?.Invoke(this, e);
            return e.Cancel;
        }

        /// <summary>
        /// Ottiene un riferimento alla finestra contenente la pagina.
        /// </summary>
        protected LiteNavigationWindow GetWindow()
            => Window.GetWindow(this) as LiteNavigationWindow;
    }
}
