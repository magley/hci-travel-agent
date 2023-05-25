using Microsoft.EntityFrameworkCore;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using YouTravel.Model;

namespace YouTravel.Agent
{
	public partial class PlacesList : Page
	{
		private TravelContext _ctx = new();
		public ObservableCollection<Place> Places { get; set; } = new();

		public PlacesList()
		{
			InitializeComponent();
			DataContext = this;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			InitDbContext();
			InitMapsApi();
		}

		private void InitDbContext()
		{
			LoadPlaces();
		}

		private void LoadPlaces()
		{
			_ctx.Places.Load();
			Places = _ctx.Places.Local.ToObservableCollection();
			lstPlaces.DataContext = Places;

			if (Places.Count > 0)
			{
				lstPlaces.SelectedIndex = 0;
			}

			PinToSelectedListItem();
		}

		private void InitMapsApi()
		{
			string mapsApiKey = File.ReadAllText("Data/MapsApiKey.apikey");
			MyMap.CredentialsProvider = new ApplicationIdCredentialsProvider(mapsApiKey);
			MyMap.ZoomLevel = 8;
		}

		private void lstPlaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			PinToSelectedListItem();
		}

		private void PinToSelectedListItem()
		{
			if (Places.Count == 0 || lstPlaces.SelectedIndex == -1)
			{
				return;
			}

			var selectedPlace = Places[lstPlaces.SelectedIndex];
			var location = new Location(selectedPlace.Lat, selectedPlace.Long);
			DrawImage(selectedPlace);
			MyMap.Center = location;
		}

		private void DrawImage(Place place)
		{
			var location = new Location(place.Lat, place.Long);
			MyMap.Children.Clear();

			MapLayer mapLayer = new();
			Image myPushPin = new()
			{
				Source = new BitmapImage(new Uri(GetPinIconUriString(place.Type), UriKind.Absolute)),
				Width = 48,
				Height = 48
			};
			mapLayer.AddChild(myPushPin, location, PositionOrigin.Center);
			MyMap.Children.Add(mapLayer);
		}

		private string GetPinIconUriString(PlaceType type)
		{
			string fname = "";
			switch (type)
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


		private void btnEditPlace_Click(object sender, RoutedEventArgs e)
		{
			Button btn = (Button)sender;
			Place place = (Place)btn.DataContext;

			((AgentMainWindow)Window.GetWindow(this)).OpenPage(new LocationAdd(place));

			Refresh(Places);
		}

		private void btnRemovePlace_Click(object sender, RoutedEventArgs e)
		{

		}

		public static void Refresh<T>(ObservableCollection<T> value)
		{
			// This is for testing.
			// You would call Refresh(Places) after changing an existing Place in Places.
			// HACK: This is a terrible hack. I wish WPF had normal binding.
			// TODO: Do we need this? Now that we have pages, it'll refresh every time we change somethign.
			CollectionViewSource.GetDefaultView(value).Refresh();
		}
	}
}
