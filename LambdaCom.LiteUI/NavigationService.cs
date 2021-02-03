using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace LambdaCom.LiteUI
{
    /// <summary>
    /// Collezione di parametri passati dal <see cref="NavigationService"/>.
    /// </summary>
    public sealed class NavigationParams
    {
        private readonly Dictionary<string, object> extras = new Dictionary<string, object>();

        /// <summary>
        /// Aggiunge un parametro.
        /// </summary>
        /// <param name="key">La chiave del parametro.</param>
        /// <param name="value">Il valore del parametro.</param>
        public NavigationParams Add(string key, object value)
        {
            extras.Add(key, value);
            return this;
        }

        /// <summary>
        /// Preleva un parametro o un valore di default.
        /// </summary>
        /// <param name="key">La chiave del parametro.</param>
        /// <param name="defaultValue">Il valore di default nel caso in cui il parametro sia assente.</param>
        /// <returns></returns>
        public object Get(string key, object defaultValue)
        {
            if (extras.TryGetValue(key, out var value))
                return value;
            else
                return defaultValue;
        }

        /// <summary>
        /// Preleva un parametro o un valore di default.
        /// </summary>
        /// <typeparam name="T">Il tipo del parametro.</typeparam>
        /// <param name="key">La chiave del parametro.</param>
        /// <param name="defaultValue">Il valore di default nel caso in cui il parametro sia assente.</param>
        public T Get<T>(string key, object defaultValue)
            => (T)Get(key, defaultValue);

        /// <summary>
        /// Preleva un parametro o il valore di default di quel tipo.
        /// </summary>
        /// <typeparam name="T">Il tipo del parametro.</typeparam>
        /// <param name="key">La chiave del parametro.</param>
        /// <param name="defaultValue">Il valore di default nel caso in cui il parametro sia assente.</param>
        public T Get<T>(string key)
            => (T)Get(key, default(T));
    }

    public sealed class NavigationService
    {
        private readonly LiTENavigationWindow window;
        private readonly List<LiTEPage> history;
        private readonly Dictionary<string, LiTEPage> singletons;

        private LiTEPage current;
        private bool saveCurrent = true;

        internal NavigationService(LiTENavigationWindow window)
        {
            this.window = window;
            history = new List<LiTEPage>();
            singletons = new Dictionary<string, LiTEPage>();
        }

        private void LoadCurrent(LiTEPage page)
        {
            // Imposta nuova pagina come corrente
            current = page;

            // Carica pagina in finestra e binda a titolo pagina in modo da reagire ai cambiamenti 
            window.PageTitle.SetBinding(ContentControl.ContentProperty, new Binding(nameof(page.Title))
            {
                Source = page
            });

            window.Back.IsEnabled = CanGoBack();
            window.Content = page;

            // Imposta la toolbar della pagina sulla finestra
            window.Toolbar = page.Toolbar;
        }

        internal bool CancelClosing()
        {
            // Comunica alla LiTENavigationWindow se va annullata la chiusura
            return current?.CallCancelNavigation() ?? false;
        }

        internal void Dispose()
        {
            // Disponi la pagina corrente, quelle nella cronologia ed i singleton
            if (current is IDisposable disposable)
                disposable.Dispose();

            foreach (var page in history)
                if (page is IDisposable disposableSingleton)
                    disposableSingleton.Dispose();

            foreach (var page in singletons.Values)
                if (page is IDisposable disposableSingleton)
                    disposableSingleton.Dispose();
        }

        /// <summary>
        /// <see langword="true"/> se è presente almeno una pagina nella cronologia di navigazione, altrimenti <see langword="false"/>.
        /// </summary>
        public bool CanGoBack()
        {
            return history.Count > 0;
        }

        /// <summary>
        /// Ritorna alla pagina precedente nella cronologia di navigazione.<para/>Se la pagina implementa <see cref="IDisposable"/> verrà disposta automaticamente.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">La cronologia di navigazione è vuota.</exception>
        public void GoBack()
        {
            if (current != null)
            {
                // Se la pagina non va abbandonata termina
                if (current.CallCancelNavigation())
                    return;

                // Se la pagina corrente non è un singleton ed implementa IDisposable eseguilo
                if (!singletons.ContainsValue(current) && current is IDisposable disposable)
                    disposable.Dispose();
            }

            // Se è nella cronologia è certamente una pagina da salvare
            saveCurrent = true;

            // Preleva l'ultima pagina dalla cronologia e caricala
            int lastIndex = history.Count - 1;

            var page = history[lastIndex];
            history.RemoveAt(lastIndex);

            LoadCurrent(page);
        }

        /// <summary>
        /// Naviga ad una pagina del tipo dato con i parametri dati.
        /// </summary>
        /// <param name="type">Il tipo della pagina da aprire.</param>
        /// <param name="extras">I parametri da passare alla pagina.</param>
        /// <exception cref="ArgumentNullException">Il tipo della pagina non può essere <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Il tipo della pagina deve essere sottoclasse di <see cref="LiTEPage"/>.</exception>
        public void Navigate(Type type, NavigationParams extras = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (current != null)
            {
                // Se la pagina non va abbandonata termina
                if (current.CallCancelNavigation())
                    return;

                // Se la pagina non è da ignorare aggiungila alla cronologia
                if (saveCurrent)
                    history.Add(current);
            }

            // Crea pagina e caricala
            if (type.IsSubclassOf(typeof(LiTEPage)))
            {
                // Leggi da attributo, se presente, le impostazioni della pagina, altrimenti usa parametri di default
                saveCurrent = true;
                bool singleton = false;
                bool instantiateNew = true;

                if (Attribute.GetCustomAttribute(type, typeof(PageOptionsAttribute)) is PageOptionsAttribute attributes)
                {
                    switch (attributes.LaunchMode)
                    {
                        case PageLaunchMode.Ignore:
                            saveCurrent = false;
                            singleton = false;
                            instantiateNew = true;
                            break;
                        case PageLaunchMode.Normal:
                            saveCurrent = true;
                            singleton = false;
                            instantiateNew = true;
                            break;
                        case PageLaunchMode.SingleInstance:
                            saveCurrent = true;
                            singleton = false;
                            instantiateNew = false;
                            break;
                        case PageLaunchMode.Singleton:
                            saveCurrent = true;
                            singleton = true;
                            instantiateNew = false;
                            break;
                    }
                }

                // Carica nuova istanza di pagina o se necessario riporta in cima il singleton
                LiTEPage page;

                if (singleton)
                {
                    // Se già aperto riporta singleton, altrimenti crea
                    if (singletons.TryGetValue(type.FullName, out page))
                    {
                        page.CallRetrieved(extras ?? new NavigationParams());
                    }
                    else
                    {
                        page = (LiTEPage)Activator.CreateInstance(type);
                        page.CallCreated(extras ?? new NavigationParams());

                        singletons.Add(type.FullName, page);
                    }
                }
                else if (instantiateNew || (page = history.Where(p => p.GetType() == type).FirstOrDefault()) == null)
                {
                    page = (LiTEPage)Activator.CreateInstance(type);
                    page.CallCreated(extras ?? new NavigationParams());
                }
                else
                {
                    page.CallRetrieved(extras ?? new NavigationParams());
                }

                LoadCurrent(page);
            }
            else
                throw new ArgumentException("Type must be subclass of " + nameof(LiTEPage), nameof(type));
        }

        /// <summary>
        /// Naviga ad una pagina del tipo dato con i parametri dati.
        /// </summary>
        /// <typeparam name="T">Il tipo della pagina da aprire.</typeparam>
        /// <param name="extras">I parametri da passare alla pagina.</param>
        /// <exception cref="ArgumentNullException">Il tipo della pagina non può essere <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Il tipo della pagina deve essere sottoclasse di <see cref="LiTEPage"/>.</exception>
        public void Navigate<T>(NavigationParams extras) where T : LiTEPage, new()
            => Navigate(typeof(T), extras);

        /// <summary>
        /// Naviga ad una pagina del tipo dato con i parametri dati.
        /// </summary>
        /// <typeparam name="T">Il tipo della pagina da aprire.</typeparam>
        /// <param name="extras">I parametri da passare alla pagina.</param>
        /// <exception cref="ArgumentNullException">Il tipo della pagina non può essere <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Il tipo della pagina deve essere sottoclasse di <see cref="LiTEPage"/>.</exception>
        public void Navigate<T>(params (string key, object value)[] extras) where T : LiTEPage, new()
        {
            // Ricrea l'oggetto parametro dai valori passati
            var param = new NavigationParams();

            foreach (var (key, value) in extras)
                param.Add(key, value);

            Navigate<T>(param);
        }

        /// <summary>
        /// Naviga ad una pagina del tipo dato senza passare alcun parametro.
        /// </summary>
        /// <typeparam name="T">Il tipo della pagina da aprire.</typeparam>
        /// <exception cref="ArgumentNullException">Il tipo della pagina non può essere <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Il tipo della pagina deve essere sottoclasse di <see cref="LiTEPage"/>.</exception>
        public void Navigate<T>() where T : LiTEPage, new()
            => Navigate(typeof(T));
    }
}
