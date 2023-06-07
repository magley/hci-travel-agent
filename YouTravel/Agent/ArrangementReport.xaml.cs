using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YouTravel.Model;

namespace YouTravel.Agent
{
    public partial class ArrangementReport : Page
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public Arrangement Arrangement { get; set; }

        public ArrangementReport(Arrangement arrangement)
        {
            InitializeComponent();
            DataContext = this;
            using (var db = new TravelContext())
            {
                // NOTE: This is an issue with lazy loading: you have to explicitly
                // tell the context to fetch the other entity, too.
                Arrangement = db.Arrangements.Include(a => a.Reservations).Where(a => a.Id == arrangement.Id).First();
            }
            tbReservations.DataContext = Arrangement.Reservations;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
			Mouse.OverrideCursor = null;
		}

        private void btnPrevPage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
