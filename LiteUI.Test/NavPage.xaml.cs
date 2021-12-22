using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LiteUI.Test
{
    /// <summary>
    /// Logica di interazione per NavPage.xaml
    /// </summary>
    [PageOptions(LaunchMode = PageLaunchMode.Normal)]
    public partial class NavPage : LitePage
    {
        public NavPage()
        {
            InitializeComponent();
        }

        private void LitePage_Created(object sender, NavigationParams e)
        {
            Title = e.Get<string>("Title", "Default") + "C";
        }

        private void LitePage_Retrieved(object sender, NavigationParams e)
        {
            Title = e.Get<string>("Title", "Default") + "R";
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var me = (ListBox)sender;

            switch (me.SelectedIndex)
            {
                case 0:
                    GetWindow()?.SetTheme(LiteTheme.Light);
                    return;
                case 1:
                    GetWindow()?.SetTheme(LiteTheme.Dark);
                    return;
                case 2:
                    GetWindow()?.SetTheme(new(
                        Color.FromRgb(0x00, 0xFF, 0x00),
                        Color.FromRgb(0xFF, 0x00, 0x00),
                        Color.FromRgb(0x00, 0x00, 0x00),
                        Color.FromRgb(0x00, 0x00, 0x00),
                        Color.FromRgb(0xFF, 0xFF, 0x00)
                    ));
                    break;
                case 3:
                    GetWindow()?.SetTheme(new(
                        Color.FromRgb(0x00, 0xFF, 0xFF),
                        Color.FromRgb(0x00, 0x7F, 0x7F),
                        Color.FromRgb(0x00, 0x00, 0x00),
                        Color.FromRgb(0x00, 0x00, 0x00),
                        Color.FromRgb(0x00, 0xFF, 0xFF)
                    ));
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LiteMessageBox.Show(GetWindow(), "Ciao", MessageBoxImage.Question, MessageBoxButton.OK);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //popup.IsOpen = !popup.IsOpen;

            /*var Arr = Enum.GetValues<WindowBarStyle>();
            int j = Array.IndexOf(Arr, GetWindow().BarStyle) + 1;

            GetWindow().BarStyle = (Arr.Length == j) ? Arr[0] : Arr[j];*/

            NavigationService.Navigate<NavPage>(("Title", Title));
        }

        private void tbb_Click(object sender, RoutedEventArgs e)
        {
            pop.IsOpen = !pop.IsOpen;
        }
    }
}
