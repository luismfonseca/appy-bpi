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
    public partial class Register : PhoneApplicationPage
    {
        public Register()
        {
            InitializeComponent();
        }

        private async void OnRegisterTap(object sender, EventArgs e)
        {
            var user = new UserRegister()
            {
                name = name.Text,
                email = email.Text,
                password = password.Password
            };
            progressBar.IsIndeterminate = true;
            progressBar.Visibility = Visibility.Visible;
            var result = await API.GetAsync<UserRegisterResponse>(API.Actions.Register, user);
            progressBar.IsIndeterminate = false;
            progressBar.Visibility = Visibility.Collapsed;
            if (string.IsNullOrEmpty(result.UserRegisterResult))
            {
                MessageBox.Show("An account for the given email address is already created.");
            }
            else
            {
                Settings.UserID = result.UserRegisterResult;
                NavigationService.Navigate(new Uri("/Menu.xaml", UriKind.Relative));
            }
        }

        private void textChanged(object sender, TextChangedEventArgs e)
        {
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled =
                name.Text.Length > 0 && email.Text.Length > 0 && password.Password.Length > 0;
        }

        private void passwordChanged(object sender, RoutedEventArgs e)
        {
            textChanged(sender, null);
        }
    }
}