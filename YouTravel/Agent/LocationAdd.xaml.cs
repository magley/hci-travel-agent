using Microsoft.Maps.MapControl.WPF;
using System;
using System.IO;
using System.Windows;

namespace YouTravel.Agent
{
    public partial class LocationAdd : Window
    {
        private Location? location;

        public LocationAdd(Location? loc = null)
        {
            InitializeComponent();
			CenterWindow();

			location = loc;

            Console.WriteLine($"Location {loc.Latitude}, {loc.Longitude}");
        }

		private void CenterWindow()
		{
			WindowStartupLocation = WindowStartupLocation.CenterOwner;
			Owner = Application.Current.MainWindow;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			InitMapsApi();
		}

		private void InitMapsApi()
		{
			string mapsApiKey = File.ReadAllText("Data/MapsApiKey.apikey");
			MyMap.CredentialsProvider = new ApplicationIdCredentialsProvider(mapsApiKey);
		}
	}
}
