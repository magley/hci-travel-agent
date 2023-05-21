using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YouTravel.Model;

namespace YouTravel.Agent
{
    public partial class ArrangementEdit : Window
    {
        private Arrangement arrangement;
        public ArrangementEdit(Arrangement arr)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = Application.Current.MainWindow;

            arrangement = arr;
            DataContext = arrangement;

            InitCalendarRange();
        }

        private void InitCalendarRange()
        {
            arrangementCalendar.SelectedDates.Add(arrangement.Start);
            arrangementCalendar.SelectedDates.Add(arrangement.End);
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Calendar calendar = (Calendar)sender;

            try
            {
                DateTime d1 = calendar.SelectedDates[0];
                DateTime d2 = calendar.SelectedDates.Last();
                SetArrangementDates(d1, d2);
            } catch (ArgumentOutOfRangeException)
            {
                return;
            }
        }

        private void SetArrangementDates(DateTime start, DateTime end)
        {
            if (start > end)
            {
                SetArrangementDates(end, start);
                return;
            }

            Console.WriteLine($"{start} {end}");
        }
    }
}
