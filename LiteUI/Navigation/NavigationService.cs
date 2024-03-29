﻿using LiteUI.Controls;
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
        private Page current;

        internal NavigationService(Action<Page> onLoadPageCallBack)
        {
            _onLoadPageCallBack = onLoadPageCallBack;
            history = new Stack<Page>();
        }

        private void LoadCurrent(Page page)
        {
            // Salva pagina attuale e scarica navigationservice se necessario
            var old = current;
            old?.SetNavigationService(null);

            // Imposta la nuova pagina come corrente, ingetta navigationservice e chiama callback
            current = page;
            current.SetNavigationService(this);
            _onLoadPageCallBack(page);

            // Esegui evento pagina abbandonata su vecchia se necessario
            old?.CallLeft();
        }

        internal bool CancelLeaving()
        {
            // Comunica se sia necessario restare sulla pagina corrente
            return current?.CallLeaving() ?? false;
        }

        internal void Dispose()
        {
            // Disponi la pagina corrente e quelle nella cronologia
            if (current is IDisposable disposableCurrent)
                disposableCurrent.Dispose();

            while (history.Count > 0)
                if (history.Pop() is IDisposable disposablePage)
                    disposablePage.Dispose();
        }

        /// <summary>
        /// La pagina attualmente visualizzata.
        /// </summary>
        public Page CurrentPage => current;

        /// <summary>
        /// Controlla se sia presente almeno una pagina nella cronologia di navigazione.
        /// </summary>
        public bool CanGoBack => history.Count > 0;

        /// <summary>
        /// Ritorna alla pagina precedente nella cronologia di navigazione con i parametri dati.<para/>Se la pagina implementa <see cref="IDisposable"/> verrà disposta automaticamente.
        /// </summary>
        /// <param name="extras">I parametri da ritornare alla pagina.</param>
        /// <exception cref="InvalidOperationException">La cronologia di navigazione è vuota.</exception>
        public void GoBack(NavigationParams extras = null)
        {
            if (!CanGoBack)
                throw new InvalidOperationException("La cronologia di navigazione è vuota.");

            // Se la pagina non va abbandonata termina
            if (current.CallLeaving())
                return;

            // Se non sono stati passati parametri crea vuoto
            if (extras is null)
                extras = new NavigationParams();

            // Se la pagina implementa IDisposable eseguilo
            if (current is IDisposable disposable)
                disposable.Dispose();

            // Se è nella cronologia è certamente una pagina da salvare
            saveCurrent = true;

            // Preleva l'ultima pagina dalla cronologia, passa i parametri restituiti e caricala
            var page = history.Pop();
            page.CallRetrieved(extras);
            LoadCurrent(page);
        }

        /// <summary>
        /// Ritorna alla pagina precedente nella cronologia di navigazione con i parametri dati.<para/>Se la pagina implementa <see cref="IDisposable"/> verrà disposta automaticamente.
        /// </summary>
        /// <param name="id">L'Id della transazione</param>
        /// <param name="extras">I parametri da passare alla pagina.</param>
        /// <exception cref="InvalidOperationException">La cronologia di navigazione è vuota.</exception>
        public void GoBack(int id, params (int key, object value)[] extras)
        {
            // Ricrea l'oggetto parametro dai valori passati
            var param = new NavigationParams(id);

            foreach (var (key, value) in extras)
                param.Add(key, value);

            GoBack(param);
        }

        /// <summary>
        /// Ritorna alla pagina precedente nella cronologia di navigazione con i parametri dati.<para/>Se la pagina implementa <see cref="IDisposable"/> verrà disposta automaticamente.
        /// </summary>
        /// <param name="extras">I parametri da passare alla pagina.</param>
        /// <exception cref="InvalidOperationException">La cronologia di navigazione è vuota.</exception>
        public void GoBack(params (int key, object value)[] extras)
        {
            // Ricrea l'oggetto parametro dai valori passati
            var param = new NavigationParams();

            foreach (var (key, value) in extras)
                param.Add(key, value);

            GoBack(param);
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
                    if (current?.GetType() == type)
                        current.CallRetrieved(extras);
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
                    if (current?.GetType() == type)
                        current.CallRetrieved(extras);
                    else if(history.Any(p => p.GetType() == type))
                    {
                        var page = current;

                        do
                        {
                            // Se la pagina non va abbandonata termina
                            if (page.CallLeaving())
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
                if (current != null)
                {
                    // Se la pagina non va abbandonata termina
                    if (current.CallLeaving())
                        return true;

                    // Se la pagina non è da ignorare aggiungila alla cronologia
                    if (saveCurrent)
                        history.Push(current);
                }

                return false;
            }

            void CreatePage()
            {
                var page = Activator.CreateInstance(type) as Page;
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
        /// <param name="id">L'Id della transazione.</param>
        /// <param name="extras">I parametri da passare alla pagina.</param>
        public void Navigate<T>(int id, params (int key, object value)[] extras) where T : Page
        {
            // Ricrea l'oggetto parametro dai valori passati
            var param = new NavigationParams(id);

            foreach (var (key, value) in extras)
                param.Add(key, value);

            Navigate<T>(param);
        }

        /// <summary>
        /// Naviga ad una pagina del tipo dato con i parametri dati.
        /// </summary>
        /// <typeparam name="T">Il tipo della pagina da aprire.</typeparam>
        /// <param name="extras">I parametri da passare alla pagina.</param>
        public void Navigate<T>(params (int key, object value)[] extras) where T : Page
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
