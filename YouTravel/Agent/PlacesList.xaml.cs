using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using YouTravel.Model;
using YouTravel.Shared;
using YouTravel.Util;

namespace YouTravel.Agent
{
	public partial class PlacesList : Page
	{
		public Paginator<Place> Paginator = new();

		public bool ShowHotel { get; set; } = true;
		public bool ShowAttraction { get; set; } = true;
		public bool ShowRestaurant { get; set; } = true;

        public ICommand CmdFocusSearch { get; private set; }

		private Place? _selectedPlace = null;

		public PlacesList()
		{
			InitializeComponent();
			DataContext = this;

			Paginator.PropertyChanged += SetPageNavButtonsEnabled;
			Paginator.Entities.CollectionChanged += OnPlaceCollectionChanged;
			Paginator.EntitiesCurrentPage.CollectionChanged += OnPlaceCurrentPageCollectionChanged;

            CmdFocusSearch = new RelayCommand(o => FocusSearch(), o => true);

			var lat = UserConfig.Instance.StartLocation_Lat;
			var lon = UserConfig.Instance.StartLocation_Long;
			MyMap.Center = new (lat, lon);
        }

        private void FocusSearch()
        {
            this.searchBox.Focus();
			this.searchBox.SelectAll();
        }

        private void OnPlaceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			LoadPlacesCurrentPage();
		}

		private void OnPlaceCurrentPageCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			ToggleNoPagesText();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			InitDbContext();
			InitMapsApi();
			Mouse.OverrideCursor = null;
		}

		private void InitDbContext()
		{
			LoadPlaces();
		}

		private void ToggleNoPagesText()
		{
            if (Paginator.Entities.Count > 0)
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

				Paginator.Entities.Clear();
				foreach (var v in ctx.Places.Local)
				{
					Paginator.Entities.Add(v);
				}

				// SEARCH

				var afterSearch = Paginator.Entities
					.Where(x => searchBox.Text == "" || StringUtil.Compare(searchBox.Text, x.Name))
					.Where(x => (ShowHotel && x.Type == PlaceType.Hotel) || 
								(ShowAttraction &&  x.Type == PlaceType.Attraction) || 
								(ShowRestaurant && x.Type == PlaceType.Restaurant)
					)
                    .Reverse()
                    .ToList();
				Paginator.Entities.Clear();
				foreach (var v in afterSearch)
				{
					Paginator.Entities.Add(v);
				}

				// -SEARCH	

                PinToSelectedListItem();
			}
		}

		private void LoadPlacesCurrentPage()
		{
			Paginator.LoadPage();
			lstPlaces.DataContext = Paginator.EntitiesCurrentPage;
			ReselectPlace();
		}

		private void ReselectPlace()
		{
			if (_selectedPlace != null)
			{
				int index = -1;
				for (int i = 0; i < Paginator.EntitiesCurrentPage.Count; i++)
				{
					if (Paginator.EntitiesCurrentPage[i].Id == _selectedPlace.Id)
					{
						index = i;
					}
				}

				if (index > -1)
				{
					lstPlaces.SelectedIndex = index;
				}
			}

			if (Paginator.EntitiesCurrentPage.Count > 0 && _selectedPlace == null)
			{
				lstPlaces.SelectedIndex = 0;
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

			try
			{
				_selectedPlace = Paginator.EntitiesCurrentPage[lstPlaces.SelectedIndex];
			}
			catch (ArgumentOutOfRangeException)
			{
			}
		}

		private void PinToSelectedListItem()
		{
			MapUtil.DrawPinOnMapBasedOnList(Paginator.Entities, lstPlaces, MyMap, true);
		}

		private void btnEditPlace_Click(object sender, RoutedEventArgs e)
		{
			Button btn = (Button)sender;
			Place place = (Place)btn.DataContext;

			((AgentMainWindow)Window.GetWindow(this)).OpenPage(new LocationAdd(place, false));
		}

		private void btnRemovePlace_Click(object sender, RoutedEventArgs e)
		{
			bool confirmed = false;
            ConfirmBox confirmBox = new("Are you sure you want to delete this place?", "Delete confirmation", "Delete", "Cancel", ConfirmBox.ConfirmBoxIcon.QUESTION);
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

				SoundUtil.PlaySound("snd_delete.wav");
				LoadPlaces();

				if (lstPlaces.SelectedIndex == -1 && Paginator.Entities.Count > 0)
				{
					lstPlaces.SelectedIndex = 0;
				}
				if (Paginator.Entities.Count == 0)
				{
					MapUtil.ClearPins(MyMap);
					MapUtil.DrawPinOnMapBasedOnList(Paginator.Entities, lstPlaces, MyMap, true);
				}
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

		private void btnPrevPage_Click(object sender, RoutedEventArgs e)
		{
			Paginator.PageIndex--;
			LoadPlacesCurrentPage();
		}

		private void btnNextPage_Click(object sender, RoutedEventArgs e)
		{
			Paginator.PageIndex++;
			LoadPlacesCurrentPage();
		}

		private void SetPageNavButtonsEnabled(object? sender, PropertyChangedEventArgs e)
		{
			btnPrevPage.IsEnabled = Paginator.PageIndex > 1;
			btnNextPage.IsEnabled = Paginator.PageIndex < Paginator.PageCount;
		}

		private void searchBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Return)
			{
				string searchQuery = searchBox.Text;
				LoadPlaces();
			}
		}
    }
}
