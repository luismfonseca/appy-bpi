using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SmartJukeBox.JsonTypes;

namespace SmartJukeBox
{
    public partial class Search : PhoneApplicationPage
    {
        public Search()
        {
            InitializeComponent();
            
            Loaded += PageLoaded;
        }

        protected void PageLoaded(object sender, RoutedEventArgs args)
        {
            tbSearch.Focus();
        }

        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbSearch.Text))
            {
                llsResults.ItemsSource = null;
                gridNoResults.Visibility = Visibility.Collapsed;
                return;
            }
            progressBar.IsIndeterminate = true;
            progressBar.Visibility = Visibility.Visible;

            bool success = false;
            try
            {
                var searchResponse = await API.GetAsync<SearchResponse>(API.Actions.Search, tbSearch.Text);
                searchResponse = searchResponse ?? new SearchResponse();

                llsResults.ItemsSource = searchResponse.results.artistmatches.artist;
                success = true;
            }
            catch
            {
            }
            if (success == false)
            {
                try
                {
                    var searchResponse = await API.GetAsync<SearchSingleResponse>(API.Actions.Search, tbSearch.Text);
                    searchResponse = searchResponse ?? new SearchSingleResponse();

                    llsResults.ItemsSource = new Artist[] { searchResponse.results.artistmatches.artist };
                } catch
                {
                    llsResults.ItemsSource = new Artist[0];
                }
            }

            if (llsResults.ItemsSource.Count == 0)
            {
                labelSearchString.Text = "                      " + tbSearch.Text;
                gridNoResults.Visibility = Visibility.Visible;
            }
            else
            {
                gridNoResults.Visibility = Visibility.Collapsed;
            }
            progressBar.IsIndeterminate = false;
            progressBar.Visibility = Visibility.Collapsed;
        }

        private void OnResultTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var control = (sender as FrameworkElement).DataContext as Artist;

            App.ViewModel.Bands.Insert(App.ViewModel.Bands.Count - 1, new ViewModels.BandViewModel() { Name = control.name, ImageUrl = control.image[2].text });

            NavigationService.GoBack();
            //var yahooSearchResult = ((YahooSearchResult)control.DataContext);

            //NavigationService.Navigate(new Uri("/StockInfoPage.xaml?yahooQuote.ShortName=" + yahooSearchResult.symbol, UriKind.Relative));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //NavigationService.RemoveBackEntry();
        }
    }
}