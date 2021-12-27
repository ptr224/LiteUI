using LiteUI.Controls;
using LiteUI.Navigation;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LiteUI.Test
{
    /// <summary>
    /// Logica di interazione per NavPage.xaml
    /// </summary>
    [PageOptions(LaunchMode = PageLaunchMode.Normal)]
    public partial class TestPage : Controls.Page
    {
        public TestPage()
        {
            InitializeComponent();
        }
    }
}
