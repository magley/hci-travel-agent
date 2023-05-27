using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YouTravel.Model;

namespace YouTravel.Agent
{
    public partial class ArrangementList : Page
    {
        public ObservableCollection<Arrangement> Arrangements { get; set; } = new();

        public bool ShowActive { get; set; } = true;
        public bool ShowFinished { get; set; } = true;
        public bool ShowDeleted { get; set; } = false;

		public ArrangementList()
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
            LoadArrangements();
        }

        private void LoadArrangements()
        {
            using (var ctx = new TravelContext())
            {
                ctx.Arrangements.Load();
                Arrangements = ctx.Arrangements.Local.ToObservableCollection();

                // TODO: Utilize a search query (see BtnSearch_Click).
                Arrangements = new(Arrangements.Where(x => ShowActive || x.Id % 2 == 0)); // Testing out filtering observable collections

                arrangementsList.DataContext = Arrangements;
            }
        }

        private void EditArrangement_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Arrangement arr= (Arrangement)button.DataContext;

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
	}
}
