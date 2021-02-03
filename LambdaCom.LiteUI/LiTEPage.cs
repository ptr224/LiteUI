using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace LambdaCom.LiteUI
{
    /// <summary>
    /// Descrive le modalità di lancio di una pagina.
    /// </summary>
    public enum PageLaunchMode
    {
        /// <summary>
        /// Viene creata una nuova pagina che non compare nella cronologia.
        /// </summary>
        Ignore,

        /// <summary>
        /// Viene creata una nuova pagina e salvata nella cronologia.
        /// </summary>
        Normal,

        /// <summary>
        /// Se la pagina è già aperta e nella cronologia riporta la stessa istanza.
        /// </summary>
        SingleInstance,

        /// <summary>
        /// Riporta sempre la stessa istanza, indipendentemente dal fatto che si trovi nella cronologia o meno.
        /// </summary>
        Singleton
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
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title),
            typeof(string), typeof(LitePage), new FrameworkPropertyMetadata(nameof(LitePage), FrameworkPropertyMetadataOptions.AffectsRender));

        [Bindable(true)]
        [Category(nameof(LitePage))]
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty ToolbarProperty = DependencyProperty.Register(nameof(Toolbar),
            typeof(ToolbarItemsCollection), typeof(LitePage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, ToolbarChanged));

        [Bindable(true)]
        [Category(nameof(LitePage))]
        public ToolbarItemsCollection Toolbar
        {
            get => (ToolbarItemsCollection)GetValue(ToolbarProperty);
            set => SetValue(ToolbarProperty, value);
        }

        private static void ToolbarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = ((LitePage)d).GetWindow();
            if (window != null)
                window.Toolbar = (ToolbarItemsCollection)e.NewValue;
        }

        /// <summary>
        /// Ottiene un riferimento alla finestra contenente la pagina.
        /// </summary>
        protected LiteNavigationWindow GetWindow()
            => (LiteNavigationWindow)Window.GetWindow(this);

        /// <summary>
        /// L'oggetto <see cref="NavigationService"/> che la pagina sta usando per supportare la navigazione o <see langword="null"/> se questa non è disponibile.
        /// </summary>
        protected NavigationService NavigationService
            => GetWindow()?.NavigationService;

        // Tieni separate le chiamate interne per semplificare la signatura di Created e Retrieved
        internal void CallCreated(NavigationParams extras)
            => Created(extras);

        internal void CallRetrieved(NavigationParams extras)
            => Retrieved(extras);

        internal bool CallCancelNavigation()
            => CancelNavigation();

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
        protected virtual bool CancelNavigation()
        {
            // Di default non cancellare la navigazione
            return false;
        }
    }
}
