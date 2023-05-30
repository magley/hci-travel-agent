using Microsoft.EntityFrameworkCore;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using YouTravel.Model;
using YouTravel.Shared;
using YouTravel.Util;

namespace YouTravel.Agent
{
	public partial class PlacesList : Page
	{
		public ObservableCollection<Place> Places { get; } = new();
		public bool ShowHotel { get; set; } = true;
		public bool ShowAttraction { get; set; } = true;
		public bool ShowRestaurant { get; set; } = true;

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

				// SEARCH

				var afterSearch = Places
					.Where(x => searchBox.Text == "" || StringUtil.Compare(searchBox.Text, x.Name))
					.Where(x => (ShowHotel && x.Type == PlaceType.Hotel) || 
								(ShowAttraction &&  x.Type == PlaceType.Attraction) || 
								(ShowRestaurant && x.Type == PlaceType.Restaurant)
					)
					.ToList();
				Places.Clear();
				foreach (var v in afterSearch)
				{
					Places.Add(v);
				}

				// -SEARCH	

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
			MapUtil.DrawPinOnMapBasedOnList(Places, lstPlaces, MyMap);
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

		private void BtnSearch_Click(object sender, RoutedEventArgs e)
		{
			string searchQuery = searchBox.Text;
			Console.WriteLine(searchQuery);
			LoadPlaces();
		}

		private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
		{
			searchBox.Text = "";
			LoadPlaces();
		}

		private void CheckBox_Click(object sender, RoutedEventArgs e)
		{
			LoadPlaces();
		}
	}
}
