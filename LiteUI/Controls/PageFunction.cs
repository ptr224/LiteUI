using LiteUI.Navigation;
using System;

namespace LiteUI.Controls
{
    public class PageFunction<TResult> : Page
    {
        /// <summary>
        /// Evento chiamato quando la pagina ritorna un valore.
        /// </summary>
        public event EventHandler<ReturnEventArgs<TResult>> Returned;

        /// <summary>
        /// Ritorna il valore della pagina.
        /// </summary>
        /// <param name="result">Il valore da assegnare.</param>
        protected void Return(TResult result)
        {
            Returned?.Invoke(this, new()
            {
                Result = result
            });
        }
    }
}
