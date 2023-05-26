using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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

        public string ArrName { get { return _arrName; } set { _arrName = value; DoPropertyChanged(nameof(ArrName)); } }
        public string Description { get { return _description; } set { _description = value; DoPropertyChanged(nameof(Description)); } }
        public double Price { get { return _price; } set { _price = value; DoPropertyChanged(nameof(Price)); } }
		public string Filename { get { return _filename; } set { _filename = value; DoPropertyChanged(nameof(Filename)); } }

		public ObservableCollection<Place> ArrActivities { get; set; } = new();

        public ArrangementAdd()
        {
            InitializeComponent();
			DataContext = this;

            pages.Add(Page1);
			pages.Add(Page2);
			pages.Add(Page3);
			pages.Add(Page4);
            PageIndex = 0;

			MovePageIndex(0);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
			InitMapsApi();


            ArrActivities.Add(new Place() { Name = "dsjdhsj" });
            ArrActivities.Add(new Place() { Name = "hrkw799" });
            ArrActivities.Add(new Place() { Name = "hhrhjl3r3" });
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
					pages[i].Visibility = System.Windows.Visibility.Visible;
				}
				else
				{
					pages[i].Visibility = System.Windows.Visibility.Hidden;
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

		}

		private void btn_SelectImage_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.DefaultExt = ".jpeg";
			dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";

			Nullable<bool> result = dlg.ShowDialog();

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
    }
}
