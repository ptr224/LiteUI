using System.Collections.Generic;

namespace LiteUI.Navigation
{
    /// <summary>
    /// Collezione di parametri passati dal <see cref="NavigationService"/>.
    /// </summary>
    public sealed class NavigationParams
    {
        /// <summary>
        /// Id di default. Non usare questo valore se si vuole assegnare un'identità alla transazione.
        /// </summary>
        public const int DEFAULT_ID = -1;

        private readonly Dictionary<int, object> extras = new Dictionary<int, object>();

        /// <summary>
        /// Identificativo della transazione.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Crea una nuova transazione specificando l'Id.
        /// </summary>
        /// <param name="id">L'Id della transazione.</param>
        public NavigationParams(int id)
        {
            Id = id;
        }

        /// <summary>
        /// Crea una nuova transazione con Id di default.
        /// </summary>
        public NavigationParams() : this(DEFAULT_ID)
        { }

        /// <summary>
        /// Aggiunge un parametro.
        /// </summary>
        /// <param name="key">La chiave del parametro.</param>
        /// <param name="value">Il valore del parametro.</param>
        public NavigationParams Add(int key, object value)
        {
            extras.Add(key, value);
            return this;
        }

        /// <summary>
        /// Preleva un parametro o un valore di default.
        /// </summary>
        /// <typeparam name="T">Il tipo del parametro.</typeparam>
        /// <param name="key">La chiave del parametro.</param>
        /// <param name="defaultValue">Il valore di default nel caso in cui il parametro sia assente.</param>
        public T Get<T>(int key, T defaultValue)
        {
            if (extras.TryGetValue(key, out var value))
                return (T)value;
            else
                return defaultValue;
        }

        /// <summary>
        /// Preleva un parametro o il valore di default di quel tipo.
        /// </summary>
        /// <typeparam name="T">Il tipo del parametro.</typeparam>
        /// <param name="key">La chiave del parametro.</param>
        public T Get<T>(int key)
            => Get(key, default(T));
    }
}
