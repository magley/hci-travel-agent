using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace YouTravel.Agent
{

	public partial class Settings : Window, INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler? PropertyChanged;
		void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

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
		}

		private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var newVal = ((TreeViewItem)e.NewValue).Header.ToString();
			//var newValOtherWay = ((TreeViewItem)TreeView.SelectedItem).Header.ToString();
			SelectedSection = newVal;
			Console.WriteLine($"{newVal} {SelectedSection}");
		}
	}
}
