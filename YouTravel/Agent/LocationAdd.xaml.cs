using Microsoft.Maps.MapControl.WPF;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using YouTravel.Model;

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
		private PlaceType _type = PlaceType.Attraction;

		public double Latitude { get { return _latitude; } set { _latitude = value; DoPropertyChanged(nameof(Latitude)); MoveMapToLocation(); } }
		public double Longitude { get { return _longitude; } set { _longitude = value; DoPropertyChanged(nameof(Longitude)); MoveMapToLocation(); } }
		public string LocName { get { return _name; } set { _name = value; DoPropertyChanged(nameof(LocName)); } }
		public string Description { get { return _description; } set { _description = value; DoPropertyChanged(nameof(Description)); } }
		public PlaceType Type { get { return _type; } set { _type = value; DoPropertyChanged(nameof(Type)); DrawImage(new(Latitude, Longitude)); } }

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
			DrawImage(MyMap.Center);
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

			_latitude = latLong.Latitude; // HACK: If we changed Latitude here, then MoveMapToLocation gets called twice
										  // but during the first call we still have the old Longitude. This wouldn't be
										  // so bad if we didn't have to add pins because then 2 pins are added. Although
										  // if we allow only 1 pin at any given time, the user won't notice this effect,
										  // but if our method ever has side effects, it can cause nasty bugs.
			Longitude = latLong.Longitude;
		}

		private void DrawImage(Location where)
		{
			MyMap.Children.Clear();

			MapLayer mapLayer = new();
			Image myPushPin = new()
			{
				Source = new BitmapImage(new Uri(GetPinIconUriString(), UriKind.Absolute)),
				Width = 48,
				Height = 48
			};
			mapLayer.AddChild(myPushPin, where, PositionOrigin.Center);
			MyMap.Children.Add(mapLayer);
		}

		private string GetPinIconUriString()
		{
			string fname = "";
			switch (_type)
			{
				case PlaceType.Attraction:
					fname = "ImgAttraction.png";
					break;
				case PlaceType.Restaurant:
					fname = "ImgRestaurant.png";
					break;
				case PlaceType.Hotel:
					fname = "ImgHotel.png";
					break;
			}

			return $"pack://application:,,,/Res/{fname}";
		}
	}
}
