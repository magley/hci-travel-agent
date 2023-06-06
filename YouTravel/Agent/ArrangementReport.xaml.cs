using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YouTravel.Model;

namespace YouTravel.Agent
{
    public partial class ArrangementReport : Page
	{
        public Arrangement arrangement { get; set; }
        private IList<Reservation> Reservations { get; set; } = new List<Reservation>();

        public ArrangementReport(Arrangement arrangement)
        {
            InitializeComponent();
            DataContext = this;
            this.arrangement = arrangement;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitDbContext();
        }

        private void InitDbContext()
        {
            using (var ctx = new TravelContext())
            {
                ctx.Arrangements.Load();
                ctx.Reservations.Load();

                Reservations = (from res in ctx.Reservations.Local.ToObservableCollection()
                                where res.Arrangement.Id == arrangement.Id
                                select res).ToList();
                tbReservations.DataContext = Reservations;
            }
        }
    }
}
