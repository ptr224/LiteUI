using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace LiteUI.Controls
{
    public static class Theming
    {
        // Calcola valori per tutte le chiavi dei colori
        private static void UpdateResources(ResourceDictionary resources, Theme theme)
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
        public static void SetGlobalTheme(Theme theme)
        {
            UpdateResources(Application.Current.Resources, theme);
        }

        // Theme

        public static readonly DependencyProperty ThemeProperty = DependencyProperty.RegisterAttached(
            "Theme",
            typeof(Theme),
            typeof(Theming),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, ThemeChanged)
        );

        private static void ThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element && e.NewValue is Theme theme)
                UpdateResources(element.Resources, theme);
            else
                throw new NotSupportedException();
        }

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        [Category(nameof(LiteUI))]
        public static Theme GetTheme(DependencyObject target)
            => (Theme)target.GetValue(ThemeProperty);

        public static void SetTheme(DependencyObject target, Theme value)
            => target.SetValue(ThemeProperty, value);

        // Accented

        public static readonly DependencyProperty AccentedProperty = DependencyProperty.RegisterAttached(
            "Accented",
            typeof(bool),
            typeof(Theming),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender)
        );

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        [Category(nameof(LiteUI))]
        public static bool GetAccented(DependencyObject target)
            => (bool)target.GetValue(AccentedProperty);

        public static void SetAccented(DependencyObject target, bool value)
            => target.SetValue(AccentedProperty, value);

        // GroupPosition

        public enum GroupPosition { None, LeftTop, Top, RightTop, Left, Center, Right, LeftBottom, Bottom, RightBottom }

        public static readonly DependencyProperty GroupPositionProperty = DependencyProperty.RegisterAttached(
            "GroupPosition",
            typeof(GroupPosition),
            typeof(Theming),
            new FrameworkPropertyMetadata(GroupPosition.None, FrameworkPropertyMetadataOptions.AffectsRender)
        );

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        [Category(nameof(LiteUI))]
        public static GroupPosition GetGroupPosition(DependencyObject target)
            => (GroupPosition)target.GetValue(GroupPositionProperty);

        public static void SetGroupPosition(DependencyObject target, GroupPosition value)
            => target.SetValue(GroupPositionProperty, value);
    }
}
