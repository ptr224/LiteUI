using System;

namespace LiteUI.Navigation
{
    /// <summary>
    /// Specifica le opzioni relative alla <see cref="Controls.Page"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class PageOptionsAttribute : Attribute
    {
        /// <summary>
        /// Specifica le modalità di lancio della pagina.
        /// </summary>
        public PageLaunchMode LaunchMode { get; set; } = PageLaunchMode.Normal;
    }
}
