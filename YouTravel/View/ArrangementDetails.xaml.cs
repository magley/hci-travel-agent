using Microsoft.EntityFrameworkCore;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YouTravel.Model;
using YouTravel.Shared;
using YouTravel.Util;
using YouTravel.Util.Api;

namespace YouTravel.View
{
    /// <summary>
    /// Interaction logic for ArrangementDetails.xaml
    /// </summary>
    public partial class ArrangementDetails : Page, INotifyPropertyChanged
    {

        public ObservableCollection<Place> ArrPlaces { get; set; } = new();
        public event PropertyChangedEventHandler? PropertyChanged;
        void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        public Arrangement Arrangement { get; set; }
        private Place? _selectedPlace = null;
        public Place? SelectedPlace
        {
            get { return _selectedPlace; }
            set { _selectedPlace = value; DoPropertyChanged(nameof(SelectedPlace)); }
        }
        private readonly MapBundle mapBundle = new();

        public ArrangementDetails(Arrangement arrangement)
        {
            InitializeComponent();
            DataContext = this;
            Arrangement = arrangement;
            mapBundle.Map = TheMap;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TitleOverride.PageNameAsWords(this);
            InitDbContext();
            InitMapsApi();
            Mouse.OverrideCursor = null;
        }

        private void InitMapsApi()
        {
            TheMap.CredentialsProvider = new ApplicationIdCredentialsProvider(MapsAPI.Key);
            TheMap.ZoomLevel = 8;
            var lat = UserConfig.Instance.StartLocation_Lat;
            var lon = UserConfig.Instance.StartLocation_Long;
            TheMap.Center = new(lat, lon);
        }

        private void InitDbContext()
        {
            using var ctx = new TravelContext();
            ctx.Arrangements.Load();
            ctx.Places.Load();
            // NOTE: This is an issue with lazy loading: you have to explicitly
            // tell the context to fetch the other entity, too.
            Arrangement.Places.Clear();
            ArrPlaces.Clear();
			var arrangement = ctx.Arrangements.Include(a => a.Places).Where(a => a.Id == Arrangement.Id).First();
            foreach (var place in arrangement.Places)
            {
                Arrangement.Places.Add(place);
                ArrPlaces.Add(place);
			}

            if (lstAllPlaces.SelectedIndex == -1 && ArrPlaces.Count > 0)
            {
                lstAllPlaces.SelectedIndex = 0;
			}
        }
        private void LstAllPlaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			mapBundle.Pins = new List<PlacePinData>(PlacePinData.From(ArrPlaces));

			// "Highlight" selected pin
			foreach (PlacePinData p in mapBundle.Pins)
			{
				if (p.Place != SelectedPlace)
				{
					p.IsSpeculativePin = false;
				}
				else
				{
					p.IsSpeculativePin = true;
				}
			}

			MapUtil.Redraw(mapBundle);
		}

        private void Book_Click(object sender, RoutedEventArgs e)
        {
            if (YouTravelContext.User == null)
            {
                new OkBox("Cannot book arrangement - must be logged in to do that.", "Cannot book arrangement").ShowDialog();
                return;
            }
            var box = new BookPeople("Book arrangement for how many people?", "Book", "Book", "Cancel");
            var diagRes = box.ShowDialog();
            if (diagRes != null && diagRes == false) return;
            var numOfPeople = box.Result;
            if (numOfPeople != null)
            {
                using var ctx = new TravelContext();
                var arrangement = ctx.Arrangements.Find(Arrangement.Id);
                Debug.Assert(arrangement != null);
                var reservation = new Reservation
                {
                    TimeOfReservation = DateTime.Now,
                    ArrangementId = arrangement.Id,
                    Arrangement = arrangement,
                    Username = YouTravelContext.User?.Username,
                    NumOfPeople = int.Parse(numOfPeople),
                };
                ctx.Reservations.Load();
                ctx.Reservations.Add(reservation);
                ctx.SaveChanges();
            }
        }
    }
}
