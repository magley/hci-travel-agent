using Microsoft.EntityFrameworkCore;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using YouTravel.Model;
using YouTravel.Util;

namespace YouTravel.Agent
{
    public partial class ArrangementEdit : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));


        public bool ActivitiesViewHotel { get; set; } = true;
        public bool ActivitiesViewAttraction { get; set; } = true;
        public bool ActivitiesViewRestaurant { get; set; } = true;

        private int arrangementId;
        private string _arrName = "New Arrangement";
        private string _description = "";
        private double _price = 100;
        private string _filename = "No File Selected.";
        private string _durationText = "No Dates Set.";

        private DateTime _start = DateTime.Now;
        private DateTime _end = DateTime.Now.AddDays(1);

        public string ArrName { get { return _arrName; } set { _arrName = value; DoPropertyChanged(nameof(ArrName)); } }
        public string Description { get { return _description; } set { _description = value; DoPropertyChanged(nameof(Description)); } }
        public double Price { get { return _price; } set { _price = value; DoPropertyChanged(nameof(Price)); } }
        public string Filename { get { return _filename; } set { _filename = value; DoPropertyChanged(nameof(Filename)); } }
        public string DurationText { get { return _durationText; } set { _durationText = value; DoPropertyChanged(nameof(DurationText)); } }

        public DateTime Start { get { return _start; } set { _start = value; DoPropertyChanged(nameof(Start)); } }
        public DateTime End { get { return _end; } set { _end = value; DoPropertyChanged(nameof(End)); } }

        public ObservableCollection<Place> AllActivities { get; set; } = new();
        public ObservableCollection<Place> ArrActivities { get; set; } = new();
        private MapBundle mapBundle = new();

        public ArrangementEdit(Arrangement arr)
        {
            InitializeComponent();
            this.DataContext = this;

            arrangementId = arr.Id;
            ArrName = arr.Name;
            Description = arr.Description;
            Price = arr.Price;
            Filename = arr.ImageFname;
            Start = arr.Start;
            End = arr.End;
            mapBundle.Map = TheMap;

            using (var db = new TravelContext())
            {
                // NOTE: This is an issue with lazy loading: you have to explicitly
                // tell the context to fetch the other entity, too.
                var arrangement = db.Arrangements.Include(a => a.Places).Where(a => a.Id == arrangementId).First();

                foreach (var place in arrangement.Places)
                {
                    ArrActivities.Add(place);
                }
            }

            ArrActivities.CollectionChanged += (a, b) => DrawMap();
            ArrActivities.CollectionChanged += (a, b) => ShowPlacesLabels();
            AllActivities.CollectionChanged += (a, b) => ShowPlacesLabels();
        }

        private void DrawMap()
        {
            mapBundle.RouteLocations = ArrActivities.Select(m => new Location(m.Lat, m.Long)).ToList();
            // TODO: Fetch the actual route using Bing Maps' API.

            mapBundle.Pins = ArrActivities;
            MapUtil.Redraw(mapBundle);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleOverride.PageNameAsWords(this);
            InitForm();
            InitCalendarRange();
            InitMapsApi();
            LoadPlaces();
            SetImage(Filename);
            Mouse.OverrideCursor = null;
        }

        private void InitForm()
        {
            textboxName.Focus();
            textboxName.SelectAll();
        }

        private void InitMapsApi()
        {
            string mapsApiKey = File.ReadAllText("Data/MapsApiKey.apikey");
            TheMap.CredentialsProvider = new ApplicationIdCredentialsProvider(mapsApiKey);
            TheMap.ZoomLevel = 8;
            var lat = UserConfig.Instance.StartLocation_Lat;
            var lon = UserConfig.Instance.StartLocation_Long;
            TheMap.Center = new(lat, lon);
        }

        private void InitCalendarRange()
        {
            arrangementCalendar.SelectedDates.AddRange(Start, End);
            arrangementCalendar.DisplayDateStart = Start;
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Calendar calendar = (Calendar)sender;

            try
            {
                DateTime d1 = calendar.SelectedDates[0];
                DateTime d2 = calendar.SelectedDates.Last();
                SetArrangementDates(d1, d2);
            } catch (ArgumentOutOfRangeException)
            {
                return;
            }
        }

        private void SetArrangementDates(DateTime start, DateTime end)
        {
            if (start > end)
            {
                SetArrangementDates(end, start);
                return;
            }

            Start = start;
            End = end;

            _start = start;
            _end = end;
            DurationText = $"{_start.ToLongDateString()} - {_end.ToLongDateString()}";
        }

        private void TheMap_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true; // Prevent other events from firing up (in this case, zooming in at cursor).
            Point mousePos = e.GetPosition(TheMap);
            Location latLong = TheMap.ViewportPointToLocation(mousePos);

            ((AgentMainWindow)Window.GetWindow(this)).OpenPage(new LocationAdd(false, latLong));
        }

        private void btn_SaveDraft_Click(object sender, RoutedEventArgs e)
        {
            UpdateArrangement();
            ((AgentMainWindow)Window.GetWindow(this)).CloseMostRecentPage();
        }

        private void btn_PublishChanges_Click(object sender, RoutedEventArgs e)
        {
            UpdateArrangement();
            ((AgentMainWindow)Window.GetWindow(this)).CloseMostRecentPage();
        }

        private void UpdateArrangement()
        {
            using (var db = new TravelContext())
            {
                db.Arrangements.Load();
                db.Places.Load();

                var arrangement = db.Arrangements.Include(a => a.Places).Where(a => a.Id == arrangementId).FirstOrDefault();

                if (arrangement == null)
                {
                    Console.WriteLine("Can't do it.");
                    return;
                }

                arrangement.Name = ArrName;
                arrangement.Description = Description;
                arrangement.Price = Price;
                arrangement.ImageFname = Filename;
                arrangement.Start = Start;
                arrangement.End = End;
                arrangement.Places.Clear();

                foreach (var place in ArrActivities)
                {
                    Place placeTracked = db.Places.Find(place.Id)!;
                    arrangement.Places.Add(placeTracked);
                }

                db.Arrangements.Update(arrangement);
                db.SaveChanges();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((AgentMainWindow)Window.GetWindow(this)).CloseMostRecentPage();
        }


        private void lstAllPlaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mapBundle.Pins = new List<Place>(ArrActivities);
            mapBundle.Pins.Add((Place)lstAllPlaces.SelectedItem);
            MapUtil.Redraw(mapBundle);
        }

        private void lstArrPlaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mapBundle.Pins = ArrActivities;
            MapUtil.Redraw(mapBundle);
        }

        private void btnAddArr_Click(object sender, RoutedEventArgs e)
        {
            int selectedAvailablePlace = lstAllPlaces.SelectedIndex;

            if (selectedAvailablePlace != -1)
            {
                Place p = AllActivities[selectedAvailablePlace];
                AllActivities.Remove(p);
                ArrActivities.Add(p);
            }
        }

        private void btnRemArr_Click(object sender, RoutedEventArgs e)
        {
            int selectedAddedPlace = lstArrPlaces.SelectedIndex;

            if (selectedAddedPlace != -1)
            {
                Place p = ArrActivities[selectedAddedPlace];
                ArrActivities.Remove(p);
                AllActivities.Add(p);
            }
        }

        private void LoadPlaces()
        {
            // This is called even when we navigate back,
            // so some of the Place entities may have been
            // removed in the meantime.

            AllActivities.Clear();
            using (var db = new TravelContext())
            {
                db.Places.Load();
                foreach (var place in db.Places.Local)
                {
                    AllActivities.Add(place);
                }
            }

            // Remove places in ArrActivities that don't exist anymore.

            var okayCopy = new List<Place>();
            foreach (var place in ArrActivities)
            {
                if (AllActivities.Where(p => p.Id == place.Id).Count() > 0) // Can't do Contains(), why not?
                {
                    okayCopy.Add(place);
                }
            }
            ArrActivities.Clear();
            foreach (var place in okayCopy)
            {
                ArrActivities.Add(place);
            }

            // For all activities in ArrActivities, remove those from AllActivities.

            okayCopy = new List<Place>();
            foreach (var place in AllActivities)
            {
                if (ArrActivities.Where(p => p.Id == place.Id).Count() == 0)
                {
                    okayCopy.Add(place);
                }
            }
            AllActivities.Clear();
            foreach (var place in okayCopy)
            {
                AllActivities.Add(place);
            }

            // Filter which activities are visible. TODO: Same bug as in ArrangementAdd::LoadPlaces().

            var afterSearch = AllActivities
                    .Where(x => (ActivitiesViewHotel && x.Type == PlaceType.Hotel) ||
                                (ActivitiesViewAttraction && x.Type == PlaceType.Attraction) ||
                                (ActivitiesViewRestaurant && x.Type == PlaceType.Restaurant)
            )
                    .ToList();
            AllActivities.Clear();
            foreach (var v in afterSearch)
            {
                AllActivities.Add(v);
            }

            // Select first if possible.

            if (lstArrPlaces.SelectedIndex == -1 && ArrActivities.Count > 0)
            {
                lstArrPlaces.SelectedIndex = 0;
            }
        }

        private void ShowPlacesLabels()
        {
            lblNoAllPlaces.Visibility = AllActivities.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            lblNoArrPlaces.Visibility = ArrActivities.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btn_SelectImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new();
            dlg.DefaultExt = ".jpeg";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                SetImage(filename);
            }
        }

        private void SetImage(string fnameFull)
        {
            Filename = fnameFull;

            try
            {
                BitmapImage image = new(new Uri(fnameFull, UriKind.Absolute));
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.Freeze();
                imgImage.Source = image;
            }
            catch (UriFormatException) {
                Filename = "No File Selected.";
                imgImage.Source = null;
            }
        }

        private void CbActivity_Filter(object sender, RoutedEventArgs e)
        {
            LoadPlaces();
        }

        private void arrangementCalendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            if (Mouse.Captured is Calendar || Mouse.Captured is CalendarItem)
            {
                Mouse.Capture(null);
            }
        }
    }
}
