using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace YouTravel.Agent
{
    public partial class ArrangementAdd : Page, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

		private List<Grid> pages = new();
        public int PageIndex { get; set; } = 0;

		public string ArrName { get; set; } = "New Arrangement";
		public string Description { get; set; } = "";
		public double Price { get; set; } = 0;

		private string _filename = "No File Selected.";
		public string Filename { get { return _filename; } set { _filename = value; DoPropertyChanged(nameof(Filename)); } }

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
	}
}
