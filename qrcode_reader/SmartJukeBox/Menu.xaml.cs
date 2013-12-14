using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace SmartJukeBox
{
    public partial class Menu : PhoneApplicationPage
    {
        public Menu()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

            if (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
        }

        private void TextBlock_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            lbBands.SelectedIndex = -1;
            if (App.ViewModel.Bands.Count < 5)
            {
                NavigationService.Navigate(new Uri("/Search.xaml", UriKind.Relative));
            }
        }

        private void OnCheckInTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ReadQRCode.xaml", UriKind.Relative));
        }

        private void OnShareClick(object sender, EventArgs e)
        {
            new ShareLinkTask()
            {
                Title = "BPI Appy Day - Smart Jukebox",
                LinkUri = new Uri("http://smartjukebox.com/bpiappyday", UriKind.Absolute),
                Message = "Hi, I'm using Smart Juke box and I'm at " + "BPI Appy Day" + " and I'm listenning to The Beatles - Carry That Weight."
            }.Show();
        }

        private void OnPanoramaSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationBar.IsVisible = panorama.SelectedIndex == 0 && App.ViewModel.SpotPivotItemVisible == Visibility.Visible;
        }
    }
}