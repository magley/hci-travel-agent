using Microsoft.Maps.MapControl.WPF;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using YouTravel.Model;
using YouTravel.Util;

namespace YouTravel.Agent
{
    public partial class LocationAdd : Page, INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler? PropertyChanged;
		void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

		private double _latitude = 0;
		private double _longitude = 0;
		private string _name = "New Location";
		private string _description = "";
		private PlaceType _type = PlaceType.Attraction;
		private int placeId = -1;

		public double Latitude { get { return _latitude; } set { _latitude = value; DoPropertyChanged(nameof(Latitude)); MoveMapToLocation(); } }
		public double Longitude { get { return _longitude; } set { _longitude = value; DoPropertyChanged(nameof(Longitude)); MoveMapToLocation(); } }
		public string LocName { get { return _name; } set { _name = value; DoPropertyChanged(nameof(LocName)); } }
		public string Description { get { return _description; } set { _description = value; DoPropertyChanged(nameof(Description)); } }
		public PlaceType Type { get { return _type; } set { _type = value; DoPropertyChanged(nameof(Type)); MoveMapToLocation(); } }

		public LocationAdd(Place place)
		{
			InitializeComponent();

			placeId = place.Id;
			Latitude = place.Lat;
			Longitude = place.Long;
			LocName = place.Name;
			Type = place.Type;

			DataContext = this;
		}

		public LocationAdd(Location? loc = null)
        {
            InitializeComponent();

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
			Place anonymous = new() { Lat = Latitude, Long = Longitude, Type = Type };
			MapUtil.DrawPin(anonymous, MyMap);
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

			// HACK: These 2 lines are a hack but we don't want MoveMapToLocation() to be called twice.
			_latitude = latLong.Latitude;
			DoPropertyChanged(nameof(Latitude));
			Longitude = latLong.Longitude;
		}

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
			SaveChanges();
        }

		private void SaveChanges()
		{
			bool creatingNew = placeId == -1;

			using (var ctx = new TravelContext())
			{
				Place place;
				if (creatingNew)
				{
                    place = new();           
				}
				else
				{
                    place = ctx.Places.Find(placeId)!;
                }

				place.Name = LocName;
				place.Lat = Latitude;
				place.Long = Longitude;
				place.Type = Type;
				// Description. Do we need it?

				if (creatingNew)
				{
                    ctx.Places.Add(place);
                }
				else
				{
                    ctx.Places.Update(place);
                }

				ctx.SaveChanges();
			}

			((AgentMainWindow)Window.GetWindow(this)).CloseMostRecentPage();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((AgentMainWindow)Window.GetWindow(this)).CloseMostRecentPage();
        }
    }
}
