using Microsoft.Maps.MapControl.WPF;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YouTravel.Model;
using YouTravel.Shared;
using YouTravel.Util;
using YouTravel.Util.Api;

namespace YouTravel.View
{
    public partial class LocationAdd : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private const string defaultLocationName = "New Location";
        private const string defaultLocationAddress = "Unknown Address";

        private double _latitude = 0;
        private double _longitude = 0;
        private string _name = defaultLocationName;
        private string _address = "";
        private string _description = "";
        private PlaceType _type = PlaceType.Attraction;
        private readonly int placeId = -1;

        public double Latitude { get { return _latitude; } set { _latitude = value; DoPropertyChanged(nameof(Latitude)); MoveMapToLocation(); } }
        public double Longitude { get { return _longitude; } set { _longitude = value; DoPropertyChanged(nameof(Longitude)); MoveMapToLocation(); } }
        public string LocName { get { return _name; } set { _name = value; DoPropertyChanged(nameof(LocName)); } }
        public string LocAddress { get { return _address; } set { _address = value; DoPropertyChanged(nameof(LocAddress)); } }
        public string Description { get { return _description; } set { _description = value; DoPropertyChanged(nameof(Description)); } }
        public PlaceType Type { get { return _type; } set { _type = value; DoPropertyChanged(nameof(Type)); MoveMapToLocation(); } }

        private readonly bool shouldReturnToPlacesList;

        public LocationAdd(bool shouldReturnToPlacesList, Place place)
        {
            InitializeComponent();

            placeId = place.Id;
            Latitude = place.Lat;
            Longitude = place.Long;
            LocName = place.Name;
            LocAddress = place.Address;
            Type = place.Type;
            Description = place.Description;

            MyMap.Center = new(Latitude, Longitude);
            this.shouldReturnToPlacesList = shouldReturnToPlacesList;
            DataContext = this;
        }

        public LocationAdd(bool shouldReturnToPlacesList, Location? loc = null)
        {
            InitializeComponent();

            if (loc != null)
            {
                Latitude = loc.Latitude;
                Longitude = loc.Longitude;
            }
            else
            {
                Latitude = UserConfig.Instance.StartLocation_Lat;
                Longitude = UserConfig.Instance.StartLocation_Long;
            }
            FetchAndSetLocAddress();

            MyMap.Center = new(Latitude, Longitude);
            this.shouldReturnToPlacesList = shouldReturnToPlacesList;
            DataContext = this;
        }

        private async void FetchAndSetLocAddress()
        {
            LocAddress = await LocationRecognition.FetchFirstLocationAddressAsync(Latitude, Longitude) ?? defaultLocationAddress;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleOverride.PageNameAsWords(this);
            InitMapsApi();
            InitForm();
            Mouse.OverrideCursor = null;
        }

        private void InitForm()
        {
            textboxName.Focus();
            textboxName.SelectAll();
        }

        private void MoveMapToLocation()
        {
            Place anonymous = new() { Lat = Latitude, Long = Longitude, Type = Type };
            MapUtil.DrawPin(anonymous, MyMap, false);
        }

        private void InitMapsApi()
        {
            MyMap.CredentialsProvider = new ApplicationIdCredentialsProvider(MapsAPI.Key);
            MyMap.ZoomLevel = 8;
        }

        private void MyMap_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            Point mousePos = e.GetPosition(MyMap);
            Location latLong = MyMap.ViewportPointToLocation(mousePos);

            // HACK: These 2 lines are a hack but we don't want MoveMapToLocation() to be called twice.
            _latitude = latLong.Latitude;
            DoPropertyChanged(nameof(Latitude));
            Longitude = latLong.Longitude;

            FetchAndSetLocAddress();
        }

        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
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
                place.Address = LocAddress;
                place.Lat = Latitude;
                place.Long = Longitude;
                place.Type = Type;
                place.Description = Description;

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

            string verb = creatingNew ? "added" : "updated";
            string verb2 = creatingNew ? "Add" : "Update";

            new ConfirmBox($"Place \"{LocName}\" {verb}.", $"{verb2} place", "OK", null, ConfirmBox.ConfirmBoxIcon.INFO).ShowDialog();

            if (shouldReturnToPlacesList)
            {
                ((MainWindow)Window.GetWindow(this)).OpenPage(new PlacesList());
            }
            ((MainWindow)Window.GetWindow(this)).CloseMostRecentPage();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).CloseMostRecentPage();
        }
    }
}
