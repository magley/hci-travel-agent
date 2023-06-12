using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static YouTravel.Shared.ConfirmBox;

namespace YouTravel.Shared
{
    /// <summary>
    /// Interaction logic for BookPeople.xaml
    /// </summary>
    public partial class BookPeople : Window
    {
        public string? Result { get; set; }
        public string MessageBody { get; set; }
        public string? YesText { get; set; }
        public string? NoText { get; set; }
        private readonly ConfirmBoxIcon icon;
        public BookPeople(string messageBody, string windowTitle, string? yesText, string? noText)
        {
            InitializeComponent();
            DataContext = this;
            MessageBody = messageBody;
            Title = windowTitle;

            Application curApp = Application.Current;
            Window mainWindow = curApp.MainWindow;
            this.Owner = mainWindow;
            this.icon = ConfirmBox.ConfirmBoxIcon.QUESTION;
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            Result = null;
            Close();
        }
    }
}
