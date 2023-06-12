using Microsoft.EntityFrameworkCore;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            var arrangement = ctx.Arrangements.Include(a => a.Places).Where(a => a.Id == Arrangement.Id).First();
            foreach (var place in arrangement.Places)
            {
                Arrangement.Places.Add(place);
            }
        }
        private void LstAllPlaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mapBundle.Pins = new List<PlacePinData>(PlacePinData.From(Arrangement.Places));
            if (SelectedPlace != null)
            {
                mapBundle.Pins.Add(new PlacePinData(SelectedPlace));
            }
            MapUtil.Redraw(mapBundle);
        }

        private void Book_Click(object sender, RoutedEventArgs e)
        {
            var res = new ConfirmBox("Book arrangement?", "Book", "Yes", "No", ConfirmBox.ConfirmBoxIcon.QUESTION).ShowDialog();
            if (res ?? false)
            {
                using var ctx = new TravelContext();
                var reservation = new Reservation
                {
                    TimeOfReservation = DateTime.Now,
                    Arrangement = Arrangement,
                    Username = YouTravelContext.User?.Name,
                    NumOfPeople = 1,
                };
                ctx.Reservations.Add(reservation);
                ctx.SaveChanges();
            }
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
