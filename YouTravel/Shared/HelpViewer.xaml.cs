using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace YouTravel.Shared
{
    public partial class HelpViewer : Window
    {
        public HelpViewer(string key)
        {
            InitializeComponent();
            UpdateNavButtonEnabled();

            string currentDirectory = Directory.GetCurrentDirectory();
            string path = $"{currentDirectory}/Help/{key}.html";

            if (!File.Exists(path))
            {
                key = "error";
            }
            path = $"{currentDirectory}/Help/{key}.html";

            Uri uri = new($"file:///{path}");
            wbHelp.Navigate(uri);
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            if (wbHelp.CanGoBack)
            {
                wbHelp.GoBack();
            }
            UpdateNavButtonEnabled();
        }

        private void GoForward(object sender, RoutedEventArgs e)
        {
            if (wbHelp.CanGoForward)
            {
                wbHelp.GoForward();
            }
            UpdateNavButtonEnabled();
        }

        private void UpdateNavButtonEnabled()
        {
            BtnBack.IsEnabled = (wbHelp != null) && wbHelp.CanGoBack;
            BtnForward.IsEnabled = (wbHelp != null) && wbHelp.CanGoForward;
        }

        private void WbHelp_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            UpdateNavButtonEnabled();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
