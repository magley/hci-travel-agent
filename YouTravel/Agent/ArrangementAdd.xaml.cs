using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;

namespace YouTravel.Agent
{
    public partial class ArrangementAdd : UserControl
    {
        private List<Grid> pages = new();
        public int PageIndex { get; set; } = 0;

        public ArrangementAdd()
        {
            InitializeComponent();

            pages.Add(Page1);
			pages.Add(Page2);
			pages.Add(Page3);
			pages.Add(Page4);
            PageIndex = 0;

			MovePageIndex(0);
		}

		private void MovePageIndex(int delta)
		{
			PageIndex += delta;

			btnPrev.IsEnabled = PageIndex > 0;
			btnNext.IsEnabled = PageIndex < pages.Count - 1;

			for (int i = 0; i < pages.Count; i++)
			{
				if (i == PageIndex)
				{
					pages[i].Visibility = System.Windows.Visibility.Visible;
				}
				else
				{
					pages[i].Visibility = System.Windows.Visibility.Hidden;
				}
			}
		}

		private void btnPrev_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			MovePageIndex(-1);
		}

		private void btnNext_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			MovePageIndex(1);
		}
	}
}
