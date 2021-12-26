using System.Collections.Generic;

namespace LiteUI.Navigation
{
    /// <summary>
    /// Collezione di parametri passati dal <see cref="NavigationService"/>.
    /// </summary>
    public sealed class NavigationParams
    {
        private readonly Dictionary<string, object> extras = new();

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
        public T Get<T>(string key)
            => (T)Get(key, default(T));
    }
}
