using LiteUI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteUI.Navigation
{
    public sealed class NavigationService
    {
        private readonly Action<Page> _onLoadPageCallBack;
        private readonly Stack<Page> history;

        private bool saveCurrent = true;

        internal NavigationService(Action<Page> onLoadPageCallBack)
        {
            _onLoadPageCallBack = onLoadPageCallBack;
            history = new Stack<Page>();
        }

        private void LoadCurrent(Page page)
        {
            // Imposta nuova pagina come corrente e carica in finestra
            Current = page;
            _onLoadPageCallBack(page);
        }

        /// <summary>
        /// Comunica se sia necessario restare sulla pagina corrente.
        /// </summary>
        /// <returns></returns>
        public bool CancelClosing()
        {
            return Current?.CallClosing() ?? false;
        }

        internal void Dispose()
        {
            // Disponi la pagina corrente e quelle nella cronologia
            if (Current is IDisposable disposableCurrent)
                disposableCurrent.Dispose();

            while (history.TryPop(out var page))
                if (page is IDisposable disposablePage)
                    disposablePage.Dispose();
        }

        /// <summary>
        /// La pagina attualmente visualizzata.
        /// </summary>
        public Page Current { get; private set; }

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
            if (!CanGoBack)
                throw new InvalidOperationException("La cronologia di navigazione è vuota.");

            // Se la pagina non va abbandonata termina
            if (Current.CallClosing())
                return;

            // Se la pagina implementa IDisposable eseguilo
            if (Current is IDisposable disposable)
                disposable.Dispose();

            // Se è nella cronologia è certamente una pagina da salvare
            saveCurrent = true;

            // Preleva l'ultima pagina dalla cronologia e caricala
            LoadCurrent(history.Pop());
        }

        /// <summary>
        /// Naviga ad una pagina del tipo dato con i parametri dati.
        /// </summary>
        /// <param name="type">Il tipo della pagina da aprire.</param>
        /// <param name="extras">I parametri da passare alla pagina.</param>
        /// <exception cref="ArgumentNullException">Il tipo della pagina non può essere <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Il tipo della pagina deve essere sottoclasse di <see cref="Page"/>.</exception>
        public void Navigate(Type type, NavigationParams extras = null)
        {
            // Leggi LaunchMode del tipo, se assente imposta su Normal
            var launchMode = Attribute.GetCustomAttribute(type, typeof(PageOptionsAttribute)) is PageOptionsAttribute attributes
                ? attributes.LaunchMode
                : PageLaunchMode.Normal;

            // Se non sono stati passati parametri crea vuoto
            if (extras is null)
                extras = new NavigationParams();

            switch (launchMode)
            {
                case PageLaunchMode.Ignore:
                    if (SaveCurrent())
                        return;

                    CreatePage();
                    saveCurrent = false;
                    break;
                case PageLaunchMode.Normal:
                    if (SaveCurrent())
                        return;

                    CreatePage();
                    saveCurrent = true;
                    break;
                case PageLaunchMode.SingleTop:
                    // Se current ha il tipo richiesto chiama direttamente
                    // altrimenti controlla che la pagina corrente si possa terminare e creala
                    if (Current?.GetType() == type)
                        Current.CallRetrieved(extras);
                    else if (SaveCurrent())
                        return;
                    else
                        CreatePage();

                    saveCurrent = true;
                    break;
                case PageLaunchMode.SingleInstance:
                    // Se current ha il tipo richiesto chiama direttamente
                    // altrimenti controlla se è presente nella cronologia e inizia a disporre le pagine
                    // altrimenti controlla che la pagina corrente si possa terminare e creala
                    if (Current?.GetType() == type)
                        Current.CallRetrieved(extras);
                    else if(history.Any(p => p.GetType() == type))
                    {
                        var page = Current;

                        do
                        {
                            // Se la pagina non va abbandonata termina
                            if (page.CallClosing())
                                return;

                            // Se la pagina implementa IDisposable eseguilo
                            if (page is IDisposable disposable)
                                disposable.Dispose();
                        }
                        while ((page = history.Pop()).GetType() != type);

                        page.CallRetrieved(extras);
                        LoadCurrent(page);
                    }
                    else if(SaveCurrent())
                        return;
                    else
                        CreatePage();

                    saveCurrent = true;
                    break;
            }

            bool SaveCurrent()
            {
                if (Current is not null)
                {
                    // Se la pagina non va abbandonata termina
                    if (Current.CallClosing())
                        return true;

                    // Se la pagina non è da ignorare aggiungila alla cronologia
                    if (saveCurrent)
                        history.Push(Current);
                }

                return false;
            }

            void CreatePage()
            {
                var page = (Page)Activator.CreateInstance(type);
                page.CallCreated(extras);
                LoadCurrent(page);
            }
        }

        /// <summary>
        /// Naviga ad una pagina del tipo dato con i parametri dati.
        /// </summary>
        /// <typeparam name="T">Il tipo della pagina da aprire.</typeparam>
        /// <param name="extras">I parametri da passare alla pagina.</param>
        public void Navigate<T>(NavigationParams extras) where T : Page
            => Navigate(typeof(T), extras);

        /// <summary>
        /// Naviga ad una pagina del tipo dato con i parametri dati.
        /// </summary>
        /// <typeparam name="T">Il tipo della pagina da aprire.</typeparam>
        /// <param name="extras">I parametri da passare alla pagina.</param>
        public void Navigate<T>(params (string key, object value)[] extras) where T : Page
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
        public void Navigate<T>() where T : Page
            => Navigate(typeof(T));
    }
}
