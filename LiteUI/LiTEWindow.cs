using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace LiteUI
{
    public record LiteWindowTheme(Color Active, Color Inactive, Color Background, Color AccentForeground, Color AccentBackground)
    {
        public static readonly LiteWindowTheme Light = new(
            Color.FromRgb(0x26, 0x26, 0x26),
            Color.FromRgb(0x7F, 0x7F, 0x7F),
            Color.FromRgb(0xFF, 0xFF, 0xFF),
            Color.FromRgb(0xFF, 0xFF, 0xFF),
            Color.FromRgb(0x00, 0x7A, 0xCC)
        );

        public static readonly LiteWindowTheme Dark = new(
            Color.FromRgb(0xFF, 0xFF, 0xFF),
            Color.FromRgb(0x7F, 0x7F, 0x7F),
            Color.FromRgb(0x00, 0x00, 0x00),
            Color.FromRgb(0xFF, 0xFF, 0xFF),
            Color.FromRgb(0x00, 0x96, 0xF3)
        );
    }

    public enum WindowBarStyle { Hidden, Normal, Big }

    public class LiteWindow : Window
    {
        #region Style and properties

        static LiteWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LiteWindow), new FrameworkPropertyMetadata(typeof(LiteWindow)));
        }

        public static readonly DependencyProperty IsFullscreenProperty = DependencyProperty.Register(
            nameof(IsFullscreen),
            typeof(bool),
            typeof(LiteWindow),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender, FullscreenChanged)
        );

        [Bindable(true)]
        [Category(nameof(LiteWindow))]
        public bool IsFullscreen
        {
            get => (bool)GetValue(IsFullscreenProperty);
            set => SetValue(IsFullscreenProperty, value);
        }

        private static void FullscreenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (LiteWindow)d;
            window.MaxHeight = (bool)e.NewValue ? double.PositiveInfinity : SystemParameters.WorkArea.Height + 8;

            if (window.WindowState == WindowState.Maximized)
            {
                window.WindowState = WindowState.Minimized;
                window.WindowState = WindowState.Maximized;
            }
        }

        public static readonly DependencyProperty BarStyleProperty = DependencyProperty.Register(
            nameof(BarStyle),
            typeof(WindowBarStyle),
            typeof(LiteWindow),
            new FrameworkPropertyMetadata(WindowBarStyle.Normal, FrameworkPropertyMetadataOptions.AffectsRender)
        );

        [Bindable(true)]
        [Category(nameof(LiteWindow))]
        public WindowBarStyle BarStyle
        {
            get => (WindowBarStyle)GetValue(BarStyleProperty);
            set => SetValue(BarStyleProperty, value);
        }

        public static readonly DependencyProperty ToolbarProperty = DependencyProperty.Register(
            nameof(Toolbar),
            typeof(ToolbarItemsCollection),
            typeof(LiteWindow),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender)
        );

        [Bindable(true)]
        [Category(nameof(LiteWindow))]
        public ToolbarItemsCollection Toolbar
        {
            get => (ToolbarItemsCollection)GetValue(ToolbarProperty);
            set => SetValue(ToolbarProperty, value);
        }

        #endregion

        #region Theme

        // Calcola valori per tutte le chiavi dei colori
        private static void SetTheme(ResourceDictionary resources, LiteWindowTheme theme)
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
        public static void SetGlobalTheme(LiteWindowTheme theme)
        {
            SetTheme(Application.Current.Resources, theme);
        }

        internal LiteWindowTheme Theme { get; private set; }

        /// <summary>
        /// Imposta il tema della finestra corrente.
        /// </summary>
        /// <param name="theme">Il tema da usare.</param>
        public void SetTheme(LiteWindowTheme theme)
        {
            Theme = theme;
            SetTheme(Resources, theme);
        }

        #endregion

        public LiteWindow()
        {
            // Forza aggiornamento dell'altezza massima
            IsFullscreen = false;

            // Inizializza comandi bottoni
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, (_, __) => SystemCommands.CloseWindow(this)));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, (_, __) => SystemCommands.MinimizeWindow(this)));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, (_, __) => SystemCommands.MaximizeWindow(this)));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, (_, __) => SystemCommands.RestoreWindow(this)));
        }
    }
}