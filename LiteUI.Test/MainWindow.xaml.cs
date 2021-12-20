using System.Windows.Media;

namespace LiteUI.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : LiteNavigationWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = null;

            SetGlobalTheme(new(
                Color.FromRgb(0xFF, 0xFF, 0xFF),
                Color.FromRgb(0x7F, 0x7F, 0x7F),
                Color.FromRgb(0x27, 0x27, 0x27),
                Color.FromRgb(0x00, 0x00, 0x00),
                Color.FromRgb(0xFF, 0x7B, 0x00)
            ));
        }
    }
}
