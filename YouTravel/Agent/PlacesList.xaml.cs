using Microsoft.EntityFrameworkCore;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using YouTravel.Model;
using YouTravel.Shared;

namespace YouTravel.Agent
{
	public partial class PlacesList : Page
	{
		public ObservableCollection<Place> Places { get; set; } = new();

		public PlacesList()
		{
			InitializeComponent();
			DataContext = this;

			Places.CollectionChanged += OnPlaceCollectionChanged;
        }

        private void OnPlaceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			OnPlacesCollectionChanged();
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

		private void OnPlacesCollectionChanged()
		{
            if (Places.Count > 0)
            {
                txtNoPlaces.Visibility = Visibility.Collapsed;
                lstPlaces.Visibility = Visibility.Visible;
            }
            else
            {
                txtNoPlaces.Visibility = Visibility.Visible;
                lstPlaces.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadPlaces()
		{
			using (var ctx = new TravelContext())
			{
                ctx.Places.Load();

				Places.Clear();
				foreach (var v in ctx.Places.Local)
				{
					Places.Add(v);
                }

                lstPlaces.DataContext = Places;

				if (Places.Count > 0)
				{
					lstPlaces.SelectedIndex = 0;
                }

                PinToSelectedListItem();
			}
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
				DrawImage(null);
                return;
			}

			var selectedPlace = Places[lstPlaces.SelectedIndex];
			var location = new Location(selectedPlace.Lat, selectedPlace.Long);
			DrawImage(selectedPlace);
			MyMap.Center = location;
		}

		private void DrawImage(Place? place)
		{
			MyMap.Children.Clear();

			if (place == null)
            {
				// Setting place to null removes the pin.
                return;
			}

			var location = new Location(place.Lat, place.Long);
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
		}

		private void btnRemovePlace_Click(object sender, RoutedEventArgs e)
		{
			bool confirmed = false;
            ConfirmBox confirmBox = new("Are you sure you want to delete this place?", "Delete Confirmation");
            if (confirmBox.ShowDialog() == false)
            {
				confirmed = confirmBox.Result;
			}
			if (!confirmed)
			{
				return;
			}

			using (var ctx = new TravelContext())
			{
                Button btn = (Button)sender;
                int placeId = ((Place)btn.DataContext).Id;

				Place? place = ctx.Places.Find(placeId);
				if (place == null)
				{
					Console.WriteLine("I can't do it.");
					return;
				}

				ctx.Places.Remove(place);
				ctx.SaveChanges();

				LoadPlaces();
            }
		}
	}
}
