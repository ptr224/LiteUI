namespace LiteUI.Navigation
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
}
