using Microsoft.Maps.MapControl.WPF;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace YouTravel.Agent
{
    public partial class LocationAdd : Window, INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler? PropertyChanged;
		void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

		private double _latitude = 0;
		private double _longitude = 0;
		private string _name = "New Location";
		private string _description = "";

		public double Latitude { get { return _latitude; } set { _latitude = value; DoPropertyChanged(nameof(Latitude)); MoveMapToLocation(); } }
		public double Longitude { get { return _longitude; } set { _longitude = value; DoPropertyChanged(nameof(Longitude)); MoveMapToLocation(); } }
		public string LocName { get { return _name; } set { _name = value; DoPropertyChanged(nameof(LocName)); } }
		public string Description { get { return _description; } set { _description = value; DoPropertyChanged(nameof(Description)); } }

		public LocationAdd(Location? loc = null)
        {
            InitializeComponent();
			CenterWindow();

			if (loc != null)
			{
				Latitude = loc.Latitude;
				Longitude = loc.Longitude;
			}

			DataContext = this;
        }
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			InitMapsApi();
			InitForm();
		}

		private void InitForm()
		{
			textboxName.Focus();
			textboxName.SelectAll();
		}

		private void MoveMapToLocation()
		{
			MyMap.Center = new Location(Latitude, Longitude);
		}

		private void CenterWindow()
		{
			WindowStartupLocation = WindowStartupLocation.CenterOwner;
			Owner = Application.Current.MainWindow;
		}

		private void InitMapsApi()
		{
			string mapsApiKey = File.ReadAllText("Data/MapsApiKey.apikey");
			MyMap.CredentialsProvider = new ApplicationIdCredentialsProvider(mapsApiKey);
			MyMap.ZoomLevel = 8;
		}

		private void MyMap_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			e.Handled = true;

			Point mousePos = e.GetPosition(this);
			Location latLong = MyMap.ViewportPointToLocation(mousePos);

			Latitude = latLong.Latitude;
			Longitude = latLong.Longitude;
		}
	}
}
