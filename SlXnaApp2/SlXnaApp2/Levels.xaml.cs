using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace SlXnaApp2
{
    public partial class Levels : PhoneApplicationPage
    {
        public Levels()
        {
            InitializeComponent();
        }

        private void Button_0(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/GamePage.xaml?level=0", UriKind.Relative));
        }
        private void Button_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/GamePage.xaml?level=1", UriKind.Relative));
        }
        private void Button_2(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/GamePage.xaml?level=2", UriKind.Relative));
        }
        private void Button_3(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/GamePage.xaml?level=3", UriKind.Relative));
        }
    }
}