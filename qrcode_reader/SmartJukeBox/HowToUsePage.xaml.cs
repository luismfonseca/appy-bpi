using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;

namespace SmartJukeBox
{
    public partial class HowToUsePage : PhoneApplicationPage
    {
        public HowToUsePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationService.CanGoBack == false )//&& Settings.IsFirstTime == false)
            {
                goToMainMenu();
            }
        }

        private void OnNextTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (pHowToUse.SelectedIndex == 4)
            {
                goToMainMenu();
            }
            else
            {
                pHowToUse.SelectedIndex++;
            }
        }

        private void OnSkipTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (pHowToUse.SelectedIndex == 0)
            {
                goToMainMenu();
            }
            else
            {
                pHowToUse.SelectedIndex--;
            }
        }

        private void OnPivotSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(async delegate
            {
                double initialValue = progressBar.Value;
                double finalValue = (pHowToUse.SelectedIndex / 4.0d) * 100.0d;
                double step = (finalValue - initialValue) / 22.0d;
                for (int i = 0; i < 22; ++i)
                {
                    progressBar.Value += step;
                    await Task.Delay(1);
                }

            });
            switch (pHowToUse.SelectedIndex)
            {
                case 0:
                    bSkipOrBack.Content = "skip";
                    if ((string)bNext.Content == "done")
                    {
                        goToMainMenu();
                        pHowToUse.Visibility = Visibility.Collapsed;
                    }
                    break;
                case 1:
                    bSkipOrBack.Content = "back";
                    break;
                case 3:
                    bNext.Content = "next";
                    break;
                case 4:
                    bSkipOrBack.Content = "back";
                    bNext.Content = "done";
                    break;
            }
        }

        private void goToMainMenu()
        {
            if (NavigationService.CanGoBack)
            {
                // Probably here because user came from main menu anyways
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }
    }
}