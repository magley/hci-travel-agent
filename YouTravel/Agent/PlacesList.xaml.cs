using Microsoft.EntityFrameworkCore;
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
	public partial class PlacesList : Page, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

		public ObservableCollection<Place> Places { get; } = new();
		public ObservableCollection<Place> PlacesCurrentPage { get; } = new();

		public bool ShowHotel { get; set; } = true;
		public bool ShowAttraction { get; set; } = true;
		public bool ShowRestaurant { get; set; } = true;

		private int _pageIndex = 0;
		private int _pageCount = 0;
		public int PageIndex { get { return _pageIndex; } set { _pageIndex = value; DoPropertyChanged(nameof(PageIndex)); SetPageNavButtonsEnabled(); } }
		public int PageCount { get { return _pageCount; } set { _pageCount = value; DoPropertyChanged(nameof(PageCount)); SetPageNavButtonsEnabled(); } }
		int pageSize = 2;

        public ICommand CmdFocusSearch { get; private set; }

		private Place? _selectedPlace = null;

		public PlacesList()
		{
			InitializeComponent();
			DataContext = this;

			Places.CollectionChanged += OnPlaceCollectionChanged;
			PlacesCurrentPage.CollectionChanged += OnPlaceCurrentPageCollectionChanged;

            CmdFocusSearch = new RelayCommand(o => FocusSearch(), o => true);
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
                    .Reverse()
                    .ToList();
				Places.Clear();
				foreach (var v in afterSearch)
				{
					Places.Add(v);
				}

				// -SEARCH	

                PinToSelectedListItem();
			}
		}

		private void LoadPlacesCurrentPage()
		{
			PageCount = (int)Math.Ceiling((double)(Places.Count / (double)pageSize));
			if (PageIndex > PageCount)
			{
				PageIndex = PageCount;
			}
			if (PageIndex <= 0 && PageCount > 0)
			{
				PageIndex = 1;
			}

			int L = Math.Max(0, (PageIndex - 1) * pageSize);
			int R = Math.Min((PageIndex) * pageSize - 1, Places.Count - 1);

			PlacesCurrentPage.Clear();
			if (Places.Count != 0)
			{
				for (int i = L; i <= R; i++)
				{
					PlacesCurrentPage.Add(Places[i]);
				}
			}

			lstPlaces.DataContext = PlacesCurrentPage;

			ReselectPlace();
		}

		private void ReselectPlace()
		{
			if (_selectedPlace != null)
			{
				int index = -1;
				for (int i = 0; i < PlacesCurrentPage.Count; i++)
				{
					if (PlacesCurrentPage[i].Id == _selectedPlace.Id)
					{
						index = i;
					}
				}

				if (index > -1)
				{
					lstPlaces.SelectedIndex = index;
				}
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
				_selectedPlace = PlacesCurrentPage[lstPlaces.SelectedIndex];
			}
			catch (ArgumentOutOfRangeException)
			{
			}
		}

		private void PinToSelectedListItem()
		{
			MapUtil.DrawPinOnMapBasedOnList(Places, lstPlaces, MyMap, true);
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

				if (lstPlaces.SelectedIndex == -1 && Places.Count > 0)
				{
					lstPlaces.SelectedIndex = 0;
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
			PageIndex--;
			LoadPlacesCurrentPage();
		}

		private void btnNextPage_Click(object sender, RoutedEventArgs e)
		{
			PageIndex++;
			LoadPlacesCurrentPage();
		}

		private void SetPageNavButtonsEnabled()
		{
			btnPrevPage.IsEnabled = PageIndex > 1;
			btnNextPage.IsEnabled = PageIndex < PageCount;
		}

		private void searchBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				string searchQuery = searchBox.Text;
				LoadPlaces();
			}
		}
    }
}
