using LiteUI.Controls;
using System.Windows.Media;

namespace LiteUI.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = null;
            
            Theming.SetGlobalTheme(new()
            {
                Active = Color.FromRgb(0xFF, 0xFF, 0xFF),
                Inactive = Color.FromRgb(0x7F, 0x7F, 0x7F),
                Background = Color.FromRgb(0x27, 0x27, 0x27),
                AccentForeground = Color.FromRgb(0x00, 0x00, 0x00),
                AccentBackground = Color.FromRgb(0xFF, 0x7B, 0x00)
            });
        }
    }
}
