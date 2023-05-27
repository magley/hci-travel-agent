using Microsoft.EntityFrameworkCore;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YouTravel.Model;

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
            // Locations
        }

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			InitCalendarRange();
			InitMapsApi();
		}

		private void InitMapsApi()
		{
			string mapsApiKey = File.ReadAllText("Data/MapsApiKey.apikey");
			TheMap.CredentialsProvider = new ApplicationIdCredentialsProvider(mapsApiKey);
		}

		private void InitCalendarRange()
        {
            arrangementCalendar.SelectedDates.AddRange(Start, End);
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
        }

		private void TheMap_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            e.Handled = true; // Prevent other events from firing up (in this case, zooming in at cursor).
            Point mousePos = e.GetPosition(this);
            Location latLong = TheMap.ViewportPointToLocation(mousePos);

            ((AgentMainWindow)Window.GetWindow(this)).OpenPage(new LocationAdd(latLong));
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
                var arrangement = db.Arrangements.Find(arrangementId);

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
                // Locations

                db.Arrangements.Update(arrangement);
                db.SaveChanges();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((AgentMainWindow)Window.GetWindow(this)).CloseMostRecentPage();
        }
    }
}
