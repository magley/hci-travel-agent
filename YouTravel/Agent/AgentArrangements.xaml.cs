using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using YouTravel.Model;

namespace YouTravel.Agent
{
    public partial class AgentArrangements : Window
    {
        public ObservableCollection<Arrangement> Arrangements { get; set; } = new();
        private TravelContext _ctx = new();

        public AgentArrangements()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitDbContext();
        }

        private void InitDbContext()
        {
            _ctx.Arrangements.Load();
            Arrangements = _ctx.Arrangements.Local.ToObservableCollection();
            arrangementsList.DataContext = Arrangements;
        }

        private void ViewArrangement_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Arrangement arr= (Arrangement)button.DataContext;

            Console.WriteLine(arr.Description);
        }
    }
}
