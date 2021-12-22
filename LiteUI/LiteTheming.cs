using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LiteUI
{
    public record LiteTheme(Color Active, Color Inactive, Color Background, Color AccentForeground, Color AccentBackground)
    {
        public static readonly LiteTheme Light = new(
            Color.FromRgb(0x26, 0x26, 0x26),
            Color.FromRgb(0x7F, 0x7F, 0x7F),
            Color.FromRgb(0xFF, 0xFF, 0xFF),
            Color.FromRgb(0xFF, 0xFF, 0xFF),
            Color.FromRgb(0x00, 0x7A, 0xCC)
        );

        public static readonly LiteTheme Dark = new(
            Color.FromRgb(0xFF, 0xFF, 0xFF),
            Color.FromRgb(0x7F, 0x7F, 0x7F),
            Color.FromRgb(0x00, 0x00, 0x00),
            Color.FromRgb(0xFF, 0xFF, 0xFF),
            Color.FromRgb(0x00, 0x96, 0xF3)
        );
    }

    public static class LiteTheming
    {
        // Calcola valori per tutte le chiavi dei colori
        internal static void UpdateResources(ResourceDictionary resources, LiteTheme theme)
        {
            // Genera il colore di accento del tema
            var special = theme.Active * 0.2f + theme.Background * 0.8f;

            // Regola la palette per temi chiari e temi scuri
            var ac = System.Drawing.Color.FromArgb(0xFF, theme.Active.R, theme.Active.G, theme.Active.B);
            var bc = System.Drawing.Color.FromArgb(0xFF, theme.Background.R, theme.Background.G, theme.Background.B);
            var (accent1, accent2, window) = ac.GetBrightness() > bc.GetBrightness()
                ? (special, special * 0.2f, theme.Background) // Temi scuri
                : (theme.Background, special, special * 0.6f); // Temi chiari

            // Rimuovi eventuali trasparenze
            accent1.A = byte.MaxValue;
            accent2.A = byte.MaxValue;
            window.A = byte.MaxValue;

            // Setta colori
            resources["ActiveColor"] = new SolidColorBrush(theme.Active);
            resources["InactiveColor"] = new SolidColorBrush(theme.Inactive);
            resources["HighlightedColor"] = new SolidColorBrush(accent1);
            resources["BackgroundColor"] = new SolidColorBrush(accent2);
            resources["WindowColor"] = new SolidColorBrush(window);
            resources["AccentForeground"] = new SolidColorBrush(theme.AccentForeground);
            resources["AccentBackground"] = new SolidColorBrush(theme.AccentBackground);
        }

        /// <summary>
        /// Imposta il tema globale dell'applicazione.
        /// </summary>
        /// <param name="theme">Il tema da usare.</param>
        public static void SetGlobalTheme(LiteTheme theme)
        {
            UpdateResources(Application.Current.Resources, theme);
        }

        #region Attached Properties

        public static readonly DependencyProperty AccentedProperty = DependencyProperty.RegisterAttached(
            "Accented",
            typeof(bool),
            typeof(LiteTheming),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender)
        );

        [AttachedPropertyBrowsableForType(typeof(Button))]
        [Category(nameof(LiteTheming))]
        public static bool GetAccented(Button target)
            => (bool)target.GetValue(AccentedProperty);

        public static void SetAccented(Button target, bool value)
            => target.SetValue(AccentedProperty, value);

        #endregion
    }
}
