using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using YouTravel.Model;

namespace YouTravel.Agent
{
    public partial class ArrangementAdd : Page, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

		private List<Grid> pages = new();
        public int PageIndex { get; set; } = 0;

        private string _arrName = "New Arrangement";
        private string _description = "";
        private double _price = 100;
        private string _filename = "No File Selected.";
		private string _durationText = "No Dates Set.";

        public string ArrName { get { return _arrName; } set { _arrName = value; DoPropertyChanged(nameof(ArrName)); } }
        public string Description { get { return _description; } set { _description = value; DoPropertyChanged(nameof(Description)); } }
        public double Price { get { return _price; } set { _price = value; DoPropertyChanged(nameof(Price)); } }
		public string Filename { get { return _filename; } set { _filename = value; DoPropertyChanged(nameof(Filename)); } }
		public string DurationText { get { return _durationText; } set { _durationText = value; DoPropertyChanged(nameof(DurationText)); } }

		private DateTime? _start = null;
		private DateTime? _end = null;

        public ObservableCollection<Place> ArrActivities { get; set; } = new();

        public ArrangementAdd()
        {
            InitializeComponent();
			DataContext = this;

            pages.Add(Page1);
			pages.Add(Page2);
			pages.Add(Page3);
			pages.Add(Page4);
            pages.Add(Page5);
            PageIndex = 0;

			MovePageIndex(0);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
			InitMapsApi();

			// TODO: This is just for testing.
			try
			{
				using (var db = new TravelContext())
				{

					ArrActivities.Add(db.Places.Where(x => x.Id == 1).First());
					ArrActivities.Add(db.Places.Where(x => x.Id == 2).First());
				}
			}
			catch (System.InvalidOperationException)
			{

			}
        }

        private void MovePageIndex(int delta)
		{
			PageIndex += delta;

			btnPrev.IsEnabled = PageIndex > 0;
			btnNext.IsEnabled = PageIndex < pages.Count - 1;
			btnFinish.IsEnabled = PageIndex == pages.Count - 1;

			for (int i = 0; i < pages.Count; i++)
			{
				if (i == PageIndex)
				{
					pages[i].Visibility = Visibility.Visible;
				}
				else
				{
					pages[i].Visibility = Visibility.Hidden;
				}
			}
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
		}

        private void InitMapsApi()
        {
            string mapsApiKey = File.ReadAllText("Data/MapsApiKey.apikey");
            TheMap.CredentialsProvider = new ApplicationIdCredentialsProvider(mapsApiKey);
        }

        private void TheMap_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true; // Prevent other events from firing up (in this case, zooming in at cursor).
            Point mousePos = e.GetPosition(this);
            Location latLong = TheMap.ViewportPointToLocation(mousePos);

            ((AgentMainWindow)Window.GetWindow(this)).OpenPage(new LocationAdd(latLong));
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Calendar calendar = (Calendar)sender;

            try
            {
                DateTime d1 = calendar.SelectedDates[0];
                DateTime d2 = calendar.SelectedDates.Last();
                SetArrangementDates(d1, d2);
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

			((AgentMainWindow)Window.GetWindow(this)).CloseMostRecentPage();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((AgentMainWindow)Window.GetWindow(this)).CloseMostRecentPage();
        }
    }
}
