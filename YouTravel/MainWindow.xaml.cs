using Microsoft.EntityFrameworkCore;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using YouTravel.Model;

namespace YouTravel
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();	
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			InitMapsApi();
		}

#region Helper Funcs

		private void InitMapsApi()
		{
			string mapsApiKey = File.ReadAllText("Data/MapsApiKey.txt");
			this.MyMap.CredentialsProvider = new ApplicationIdCredentialsProvider(mapsApiKey);
		}

#endregion
	}
}
