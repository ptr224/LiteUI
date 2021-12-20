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
        /// Ottiene un riferimento alla finestra contenente la pagina.
        /// </summary>
        protected LiteNavigationWindow GetWindow()
            => (LiteNavigationWindow)Window.GetWindow(this);

        /// <summary>
        /// L'oggetto <see cref="LiteUI.NavigationService"/> che la pagina sta usando per supportare la navigazione o <see langword="null"/> se questa non è disponibile.
        /// </summary>
        public NavigationService NavigationService
            => GetWindow()?.NavigationService;

        // Tieni separate le chiamate interne per non dover dichiarare tutto internal protected
        internal void CallCreated(NavigationParams extras)
            => Created(extras);

        internal void CallRetrieved(NavigationParams extras)
            => Retrieved(extras);

        internal bool CallClosing()
            => Closing();

        /// <summary>
        /// Metodo chiamato alla creazione della pagina.
        /// </summary>
        /// <param name="extras">I parametri passati dalla chiamata.</param>
        protected virtual void Created(NavigationParams extras)
        {
            // Non fare nulla di default
        }

        /// <summary>
        /// Metodo eseguito quando la pagina viene richiamata.
        /// </summary>
        /// <param name="extras">I parametri passati dalla chiamata.</param>
        protected virtual void Retrieved(NavigationParams extras)
        {
            // Non fare nulla di default
        }

        /// <summary>
        /// Metodo eseguito quando la pagina sta per essere lasciata.<para/>
        /// Ritornare <see langword="true"/> se non si vuole che la pagina venga abbandonata.
        /// </summary>
        protected virtual bool Closing()
        {
            // Di default non cancellare la navigazione
            return false;
        }
    }
}
