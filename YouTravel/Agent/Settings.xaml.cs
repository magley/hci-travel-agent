using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YouTravel.Util;

namespace YouTravel.Agent
{
	public class StrBool
	{
		public string Str { get; set; } = "";
		public bool Bool { get; set; } = false;

		public StrBool() { }

		public StrBool(string str, bool bool_)
		{
			Str = str;
			Bool = bool_;
		}
	}

	public partial class Settings : Window, INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler? PropertyChanged;
		void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

		public UserConfig userConfig { get; } = UserConfig.Instance;

		private string _selectedSection;
		public string SelectedSection { get { return _selectedSection; } set { _selectedSection = value; DoPropertyChanged(nameof(SelectedSection)); } }
		
		public ObservableCollection<StrBool> ToolbarCheckboxItems { get; set; } = new();

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

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			InitViewModelCheckboxItems();
		}

		private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var newVal = ((TreeViewItem)e.NewValue).Header.ToString();
			SelectedSection = newVal;
			Console.WriteLine($"{newVal} {SelectedSection}");
		}

		void InitViewModelCheckboxItems()
		{
			/*
			 * I'm not sure if there's a better way to do this.
			 * We only do this once, during initialization, and
			 * pray that PossibleToolbarButtons doesn't change
			 * during runtime (it should NEVER do this, otherwise
			 * we could force the user to reset the program like
			 * what every other app does.
			 * When we tick a checkbox, ToolbarButtonVisibility_Click
			 * is called where uerConfig.ToolbarButtons, which is
			 * what we care about, gets changed. Effectively, this
			 * means that we don't need to do 2 way binding through
			 * ToolbarCheckboxItems. That's not even possible here,
			 * I think.
			 */

			foreach (var b1 in userConfig.PossibleToolbarButtons)
			{
				if (userConfig.ToolbarButtons.Contains(b1.Button))
				{
					ToolbarCheckboxItems.Add(new StrBool(b1.Name, true));
				}
				else
				{
					ToolbarCheckboxItems.Add(new StrBool(b1.Name, false));
				}
			}
		}

		private void ToolbarButtonVisibility_Click(object sender, RoutedEventArgs e)
		{
			var cb = (CheckBox)sender;
			var toolbarButtonName = cb.Content as string;

			ToolbarButton toolbarButton = userConfig.PossibleToolbarButtons.Where(tb => tb.Name == toolbarButtonName).First();
			if (cb.IsChecked == true)
			{
				if (!userConfig.ToolbarButtons.Contains(toolbarButton.Button))
				{
					userConfig.ToolbarButtons.Add(toolbarButton.Button);
				}
			}
			else
			{
				foreach (var btn in userConfig.ToolbarButtons.ToList())
				{
					if (btn == toolbarButton.Button)
					{
						userConfig.ToolbarButtons.Remove(btn);
					}
				}
			}
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			userConfig.Save();
		}
	}
}
