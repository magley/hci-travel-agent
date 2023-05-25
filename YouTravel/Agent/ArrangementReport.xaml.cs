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
        private TravelContext _ctx = new();

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
            _ctx.Arrangements.Load();
            _ctx.Reservations.Load();

            Reservations = (from res in _ctx.Reservations.Local.ToObservableCollection()
                            where res.Arrangement.Id == arrangement.Id
                            select res).ToList();
            tbReservations.DataContext = Reservations;

            Console.WriteLine(Reservations.Count);

            //Console.WriteLine(Reservations.Count + " " + _ctx.Reservations.Local.Count + " " + arrangement.Id);
        }
    }
}
