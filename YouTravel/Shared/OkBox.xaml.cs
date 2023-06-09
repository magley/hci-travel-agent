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
    /// Interaction logic for OkBox.xaml
    /// </summary>
    public partial class OkBox : Window
    {
        public string MessageBody { get; set; }

        public OkBox(string messageBody)
        {
            InitializeComponent();
            DataContext = this;
            MessageBody = messageBody;

            Owner = Application.Current.MainWindow;

            SetIcon();
        }

        private void SetIcon()
        {
            imgIconInfo.Visibility = Visibility.Collapsed;
            SystemSounds.Beep.Play();
            imgIconInfo.Visibility = Visibility.Visible;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
