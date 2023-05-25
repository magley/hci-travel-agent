using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using YouTravel.Util;

namespace YouTravel.Agent
{

	public partial class Settings : Window, INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler? PropertyChanged;
		void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

		public UserConfig userConfig { get; } = UserConfig.Instance;

		private string _selectedSection;
		public string SelectedSection { get { return _selectedSection; } set { _selectedSection = value; DoPropertyChanged(nameof(SelectedSection)); } }

		public Settings(bool openToolbarSectionOfSettings)
        {
			InitializeComponent();

			if (openToolbarSectionOfSettings)
			{
				TreeViewItem_Toolbar.IsSelected = true;
			}
			else
			{
				TreeViewItem_General.IsSelected = true;
			}
			DataContext = this;
			this.Owner = App.Current.MainWindow;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var newVal = ((TreeViewItem)e.NewValue).Header.ToString();
			SelectedSection = newVal;
			Console.WriteLine($"{newVal} {SelectedSection}");
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			userConfig.Save();
		}
	}
}
