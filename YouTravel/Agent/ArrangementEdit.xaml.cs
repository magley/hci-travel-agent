using Microsoft.Maps.MapControl.WPF;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YouTravel.Model;

namespace YouTravel.Agent
{
    public partial class ArrangementEdit : Window
    {
        private Arrangement arrangement;

        public bool ActivitiesViewHotel { get; set; } = true;
		public bool ActivitiesViewAttraction { get; set; } = true;
		public bool ActivitiesViewRestaurant { get; set; } = true;

		public ArrangementEdit(Arrangement arr)
        {
            InitializeComponent();
            CenterWindow();

			arrangement = arr;
            DataContext = arrangement;
		}

        private void CenterWindow()
        {
			WindowStartupLocation = WindowStartupLocation.CenterOwner;
			Owner = Application.Current.MainWindow;
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
            arrangementCalendar.SelectedDates.Add(arrangement.Start);
            arrangementCalendar.SelectedDates.Add(arrangement.End);
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
        }

		private void TheMap_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            e.Handled = true; // Prevent other events from firing up (in this case, zooming in at cursor).
            Point mousePos = e.GetPosition(this);
            Location latLong = TheMap.ViewportPointToLocation(mousePos);

            LocationAdd win = new(latLong);
            win.Show();
		}
	}
}
