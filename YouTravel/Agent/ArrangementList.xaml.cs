﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using YouTravel.Model;
using YouTravel.Shared;

namespace YouTravel.Agent
{
    public partial class ArrangementList : Page
    {
        public ObservableCollection<Arrangement> Arrangements { get; } = new();

        public bool ShowActive { get; set; } = true;
        public bool ShowFinished { get; set; } = true;
        public bool ShowDeleted { get; set; } = false;

		public ArrangementList()
        {
            InitializeComponent();
            DataContext = this;
            Arrangements.CollectionChanged += OnArrangementCollectionChanged;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitDbContext();
        }

        private void InitDbContext()
        {
            LoadArrangements();
        }

        private void LoadArrangements()
        {
            using (var ctx = new TravelContext())
            {
                ctx.Arrangements.Load();

                Arrangements.Clear();
                foreach (var v in ctx.Arrangements.Local)
                {
                    Arrangements.Add(v);
                }

                // TODO: Utilize a search query (see BtnSearch_Click).
                var li = Arrangements.Where(x => ShowActive || x.Id % 2 == 0).ToList();
                Arrangements.Clear();
                foreach (var v in li)
                {
                    Arrangements.Add(v);
                }

                arrangementsList.DataContext = Arrangements;
            }
        }

        private void OnArrangementCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnArrangementCollectionChanged();
        }

        private void OnArrangementCollectionChanged()
        {
            if (Arrangements.Count > 0)
            {
                txtNoArrangements.Visibility = Visibility.Collapsed;
                arrangementsList.Visibility = Visibility.Visible;
            }
            else
            {
                txtNoArrangements.Visibility = Visibility.Visible;
                arrangementsList.Visibility = Visibility.Collapsed;
            }
        }

        private void EditArrangement_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Arrangement arr = (Arrangement)button.DataContext;

            ((AgentMainWindow)Window.GetWindow(this)).OpenPage(new ArrangementEdit(arr));
        }

        private void ArrangementReport_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Arrangement arr = (Arrangement)button.DataContext;

			((AgentMainWindow)Window.GetWindow(this)).OpenPage(new ArrangementReport(arr));
        }

        private void RemoveArrangement_Click(object sender, RoutedEventArgs e)
        {
            bool confirmed = false;
            ConfirmBox confirmBox = new("Are you sure you want to delete this arrangement?", "Delete Confirmation");
            if (confirmBox.ShowDialog() == false)
            {
                confirmed = confirmBox.Result;
            }
            if (!confirmed)
            {
                return;
            }

            using (var ctx = new TravelContext())
            {
                Button btn = (Button)sender;
                int arrId = ((Arrangement)btn.DataContext).Id;

                Arrangement? arr = ctx.Arrangements.Find(arrId);
                if (arr == null)
                {
                    Console.WriteLine("I can't do it.");
                    return;
                }

                ctx.Arrangements.Remove(arr);
                ctx.SaveChanges();

                LoadArrangements();
            }
        }

        private void CbShowActive_Click(object sender, RoutedEventArgs e)
        {
            LoadArrangements();
        }

        private void CbShowFinished_Click(object sender, RoutedEventArgs e)
        {
            LoadArrangements();
        }

        private void CbShowDeleted_Click(object sender, RoutedEventArgs e)
        {
            LoadArrangements();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = this.searchBox.Text;
            Console.WriteLine(searchQuery);
            LoadArrangements();
        }

        private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            searchBox.Text = "";
        }

		private void BtnNewArrangement_Click(object sender, RoutedEventArgs e)
		{
			Button button = (Button)sender;

			((AgentMainWindow)Window.GetWindow(this)).OpenPage(new ArrangementAdd());
		}
    }
}
