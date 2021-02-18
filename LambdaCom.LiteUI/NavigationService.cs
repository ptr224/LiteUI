using System;
using System.Collections.Generic;
using System.Linq;

namespace LambdaCom.LiteUI
{
    public sealed class NavigationService
    {
        private readonly Action<LitePage> _onLoadPageCallBack;
        private readonly List<LitePage> history;

        private LitePage current;
        private bool saveCurrent = true;

        internal NavigationService(Action<LitePage> onLoadPageCallBack)
        {
            _onLoadPageCallBack = onLoadPageCallBack;
            history = new List<LitePage>();
        }

        private void LoadCurrent(LitePage page)
        {
            // Imposta nuova pagina come corrente e carica in finestra
            current = page;
            _onLoadPageCallBack(page);
        }

        internal bool CancelClosing()
        {
            // Comunica alla finestra se va annullata la chiusura
            return current?.CallCancelNavigation() ?? false;
        }

        internal void Dispose()
        {
            // Disponi la pagina corrente, quelle nella cronologia ed i singleton
            if (current is IDisposable disposableCurrent)
                disposableCurrent.Dispose();

            foreach (var page in history)
                if (page is IDisposable disposablePage)
                    disposablePage.Dispose();
        }

        /// <summary>
        /// Controlla se sia presente almeno una pagina nella cronologia di navigazione.
        /// </summary>
        public bool CanGoBack => history.Count > 0;

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

                // Se la pagina corrente non è un singleton ancora aperto ed implementa IDisposable eseguilo
                if (!(Attribute.GetCustomAttribute(current.GetType(), typeof(PageOptionsAttribute)) is PageOptionsAttribute attributes
                        && attributes.LaunchMode == PageLaunchMode.Singleton
                        && history.Any(p => p == current))
                    && current is IDisposable disposable)
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
        /// <exception cref="ArgumentException">Il tipo della pagina deve essere sottoclasse di <see cref="LitePage"/>.</exception>
        public void Navigate(Type type, NavigationParams extras = null)
        {
            if (type is null)
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
            if (type.IsSubclassOf(typeof(LitePage)))
            {
                // Leggi da attributo, se presente, le impostazioni della pagina, altrimenti tratta come Normal
                saveCurrent = true;
                bool instantiateNew = true;

                if (Attribute.GetCustomAttribute(type, typeof(PageOptionsAttribute)) is PageOptionsAttribute attributes)
                    (saveCurrent, instantiateNew) = attributes.LaunchMode switch
                    {
                        PageLaunchMode.Ignore => (false, true),
                        PageLaunchMode.Normal => (true, true),
                        PageLaunchMode.Singleton => (true, false),
                        _ => (false, false)
                    };

                // Carica nuova istanza di pagina o se singleton riporta in cima
                LitePage page;

                if (instantiateNew || (page = history.Where(p => p.GetType() == type).FirstOrDefault()) == null)
                {
                    page = (LitePage)Activator.CreateInstance(type);
                    page.CallCreated(extras ?? new NavigationParams());
                }
                else
                {
                    page.CallRetrieved(extras ?? new NavigationParams());
                }

                LoadCurrent(page);
            }
            else
                throw new ArgumentException("Type must be subclass of " + nameof(LitePage), nameof(type));
        }

        /// <summary>
        /// Naviga ad una pagina del tipo dato con i parametri dati.
        /// </summary>
        /// <typeparam name="T">Il tipo della pagina da aprire.</typeparam>
        /// <param name="extras">I parametri da passare alla pagina.</param>
        /// <exception cref="ArgumentNullException">Il tipo della pagina non può essere <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Il tipo della pagina deve essere sottoclasse di <see cref="LitePage"/>.</exception>
        public void Navigate<T>(NavigationParams extras) where T : LitePage
            => Navigate(typeof(T), extras);

        /// <summary>
        /// Naviga ad una pagina del tipo dato con i parametri dati.
        /// </summary>
        /// <typeparam name="T">Il tipo della pagina da aprire.</typeparam>
        /// <param name="extras">I parametri da passare alla pagina.</param>
        /// <exception cref="ArgumentNullException">Il tipo della pagina non può essere <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Il tipo della pagina deve essere sottoclasse di <see cref="LitePage"/>.</exception>
        public void Navigate<T>(params (string key, object value)[] extras) where T : LitePage
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
        /// <exception cref="ArgumentException">Il tipo della pagina deve essere sottoclasse di <see cref="LitePage"/>.</exception>
        public void Navigate<T>() where T : LitePage
            => Navigate(typeof(T));
    }
}
