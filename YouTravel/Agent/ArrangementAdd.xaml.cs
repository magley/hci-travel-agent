using Microsoft.EntityFrameworkCore;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YouTravel.Model;
using YouTravel.Shared;
using YouTravel.Util;

namespace YouTravel.Agent
{
    public partial class ArrangementAdd : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private List<Grid> pages = new();
        private List<Button> steps = new();
        public int PageIndex { get; set; } = 0;

        private string _arrName = "New Arrangement";
        private string _description = "";
        private double _price = 100;
        private string _filename = "No File Selected.";
        private string _durationText = "No Dates Set.";

        public bool ActivitiesViewHotel { get; set; } = true;
        public bool ActivitiesViewAttraction { get; set; } = true;
        public bool ActivitiesViewRestaurant { get; set; } = true;

        public string ArrName { get { return _arrName; } set { _arrName = value; DoPropertyChanged(nameof(ArrName)); ValidateName(); } }
        public string Description { get { return _description; } set { _description = value; DoPropertyChanged(nameof(Description)); } }
        public double Price { get { return _price; } set { _price = value; DoPropertyChanged(nameof(Price)); ValidatePrice(); } }
        public string Filename { get { return _filename; } set { _filename = value; DoPropertyChanged(nameof(Filename)); } }
        public string DurationText { get { return _durationText; } set { _durationText = value; DoPropertyChanged(nameof(DurationText)); } }

        private DateTime? _start = null;
        private DateTime? _end = null;

        private bool returnToDashboard = false;
        private MapBundle mapBundle = new();

        public ObservableCollection<Place> AllActivities { get; set; } = new();
        public ObservableCollection<Place> ArrActivities { get; set; } = new();

        public ArrangementAdd(bool returnToMainView)
        {
            InitializeComponent();
            DataContext = this;

            pages.Add(Page1);
            pages.Add(Page2);
            pages.Add(Page3);
            pages.Add(Page4);
            pages.Add(Page5);
            PageIndex = 0;

            steps.Add(Step1);
            steps.Add(Step2);
            steps.Add(Step3);
            steps.Add(Step4);
            steps.Add(Step5);

            MovePageIndex(0);

            returnToDashboard = returnToMainView;
            mapBundle.Map = TheMap;
            ArrActivities.CollectionChanged += (a, b) => DrawMap();
            ArrActivities.CollectionChanged += (a, b) => UpdateSummaryPlacesVisibility();

            ArrActivities.CollectionChanged += (a, b) => ShowPlacesLabels();
            AllActivities.CollectionChanged += (a, b) => ShowPlacesLabels();

            UpdateSummaryPlacesVisibility();
        }

        private void ValidateName()
        {
            UpdateNavigation();
        }

        private void ValidatePrice()
        {
            UpdateNavigation();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TitleOverride.PageNameAsWords(this);
            InitForm();
            InitMapsApi();
            LoadPlaces();
            Mouse.OverrideCursor = null;
        }

        private void InitForm()
        {
            textboxName.Focus();
            textboxName.SelectAll();
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

            // Filter which activities are visible.
            // TODO: There's a bug where the filter doesn't apply to elements that are added back to
            // the AllActivities list through drag and drop (obviously, because we need to call LoadPlaces()
            // again, but we can't do it in a ArrActivities.CollectionChanged event because that happens before
            // AllActivities gets the new element, and we can't do it in AllActivities.CollectionChanged because
            // we can't modify the collection in its CollectionChange events. An idea is to have another list
            // which acts as a proxy but I don't know if it'll work.

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
        }

        private void MovePageIndex(int delta)
        {
            PageIndex += delta;
            MoveToPage(PageIndex);
        }

        private void MoveToPage(int pageIndex)
        {
            PageIndex = pageIndex; // We need this.
            UpdateNavigation();
        }

        private void UpdateNavigation()
        {
            btnPrev.IsEnabled = PageIndex > 0;
            btnNext.IsEnabled = PageIndex < pages.Count - 1 && CanGoNext(PageIndex);
            btnFinish.IsEnabled = (PageIndex == pages.Count - 1);

            for (int i = 0; i < pages.Count; i++)
            {
                if (i == PageIndex)
                {
                    pages[i].Visibility = Visibility.Visible;
                    steps[i].FontWeight = FontWeights.Bold;
                }
                else
                {
                    pages[i].Visibility = Visibility.Hidden;
                    steps[i].FontWeight = FontWeights.Normal;
                }
            }

            UpdateNavList();
        }

        private bool CanGoNext(int pageIndex)
        {
            if (pageIndex == 0)
            {
                if (_arrName.Length == 0)
                {
                    return false;
                }
                if (Price <= 0)
                {
                    return false;
                }
            }

            if (pageIndex == 1) // Calendar page.
            {
                try
                {
                    DateTime d1 = arrangementCalendar.SelectedDates[0];
                    DateTime d2 = arrangementCalendar.SelectedDates.Last();
                    return true;
                }
                catch (ArgumentOutOfRangeException)
                {
                    return false;
                }
            }

            return pageIndex < pages.Count - 1;
        }

        private void UpdateNavList()
        {
            int cantClickThisStep = steps.Count;
            for (int i = 0; i < steps.Count; i++)
            {
                if (!CanGoNext(i))
                {
                    cantClickThisStep = i + 1;
                    break;
                }
            }

            for (int i = 0; i < cantClickThisStep; i++)
            {
                steps[i].IsEnabled = true;
            }
            for (int i = cantClickThisStep; i < steps.Count; i++)
            {
                steps[i].IsEnabled = false;
            }
        }

        private void ShowPlacesLabels()
        {
            lblNoAllPlaces.Visibility = AllActivities.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            lblNoArrPlaces.Visibility = ArrActivities.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            MovePageIndex(-1);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            MovePageIndex(1);
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            CreateArrangement();
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

            BitmapImage image = new(new Uri(fnameFull, UriKind.Absolute));
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.Freeze();
            imgImage.Source = image;

            SummaryNoImage.Visibility = Visibility.Collapsed;
        }

        private void InitMapsApi()
        {
            string mapsApiKey = File.ReadAllText("Data/MapsApiKey.apikey");
            TheMap.CredentialsProvider = new ApplicationIdCredentialsProvider(mapsApiKey);
            TheMap.ZoomLevel = 8;
            var lat = UserConfig.Instance.StartLocation_Lat;
            var lon = UserConfig.Instance.StartLocation_Long;
            TheMap.Center = new(lat, lon);

            DrawMap();
        }

        private void DrawMap()
        {
            mapBundle.RouteLocations = ArrActivities.Select(m => new Location(m.Lat, m.Long)).ToList();
            // TODO: Fetch the actual route using Bing Maps' API.

            mapBundle.Pins = ArrActivities;
            MapUtil.Redraw(mapBundle);
        }

        private void TheMap_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true; // Prevent other events from firing up (in this case, zooming in at cursor).
            Point mousePos = e.GetPosition(TheMap);
            Location latLong = TheMap.ViewportPointToLocation(mousePos);

            ((AgentMainWindow)Window.GetWindow(this)).OpenPage(new LocationAdd(false, latLong));
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Mouse.Capture(null);
            Calendar calendar = (Calendar)sender;

            try
            {
                DateTime d1 = calendar.SelectedDates[0];
                DateTime d2 = calendar.SelectedDates.Last();
                SetArrangementDates(d1, d2);
                UpdateNavigation();
                //UpdateNavList();
                //btnNext.IsEnabled = PageIndex < pages.Count - 1 && CanGoNext();
            }
            catch (ArgumentOutOfRangeException)
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

            _start = start;
            _end = end;
            if (_start != null && _end != null)
            {
                DurationText = $"{_start?.ToLongDateString()} - {_end?.ToLongDateString()}";
            } 
            else
            {
                DurationText = "No Dates Set.";
            }
        }

        private void CreateArrangement()
        {
            if (_start == null || _end == null)
            {
                // Please set the date.
                Console.WriteLine("Please set the date.");
                return;
            }

            using (var db = new TravelContext())
            {
                Arrangement arr = new Arrangement();
                arr.Name = ArrName;
                arr.Description = Description;
                arr.Price = Price;
                arr.ImageFname = Filename;
                arr.Start = (DateTime)_start;
                arr.End = (DateTime)_end;

                /*
                 * NOTE: Pay good attention as to what's going on here.
                 * We use 'place', fetch its ID, fetch the entity based
                 * on that ID, and put that in arr.Places.
                 * Here's why: every Place object in ArrActivities
                 * was fetched under a different TravelContext, see
                 * Page_Loaded(). The entity itself has all the correct
                 * data, but it isn't TRACKED by THIS TravelContext (the
                 * one used in this function), so it doesn't know that
                 * ArrActivities[0] is in the context itself. It thinks
                 * it's a Place and will try to insert it into the database
                 * when we do SaveChanges(). But since that untracked entity
                 * has a primary key that already exists in the table
                 * (obviously), we get a primary key constraint violation.
                 * There are 2 ways to deal with this: one is to fetch the
                 * entity under the same TravelContext that does SaveChanges,
                 * the other way I'm not sure how it works.
                 * This isn't Spring Boot.
                 * 
                 * https://stackoverflow.com/questions/43500403
                 */
                foreach (var place in ArrActivities)
                {
                    Place placeTracked = db.Places.Find(place.Id)!;
                    arr.Places.Add(placeTracked);
                }
            
                db.Arrangements.Add(arr);
                db.SaveChanges();
            }

            new ConfirmBox($"Arrangement \"{ArrName}\" added.", "Add arrangement", "OK", null, ConfirmBox.ConfirmBoxIcon.INFO).ShowDialog();

            if (returnToDashboard)
            {
                ((AgentMainWindow)Window.GetWindow(this)).OpenPage(new ArrangementList());
            }
            ((AgentMainWindow)Window.GetWindow(this)).CloseMostRecentPage();
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

        private void CbActivity_Filter(object sender, RoutedEventArgs e)
        {
            LoadPlaces();
        }

        private void Step1_Click(object sender, RoutedEventArgs e)
        {
            Button stepButton = (Button)sender;
            for (int i = 0; i < steps.Count; i++)
            {
                if (steps[i] == stepButton)
                {
                    MoveToPage(i);
                }
            }
        }

        private void arrangementCalendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            if (Mouse.Captured is Calendar || Mouse.Captured is CalendarItem)
            {
                Mouse.Capture(null);
            }
        }

        private void UpdateSummaryPlacesVisibility()
        {
            if (ArrActivities.Count == 0)
            {
                SummaryNoPlaces.Visibility = Visibility.Visible;
                SummaryPlaces.Visibility = Visibility.Collapsed;
            }
            else
            {
                SummaryNoPlaces.Visibility = Visibility.Collapsed;
                SummaryPlaces.Visibility = Visibility.Visible;
            }
        }
    }
}
