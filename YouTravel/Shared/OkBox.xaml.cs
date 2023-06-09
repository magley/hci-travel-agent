using System.Media;
using System.Windows;
using System.Windows.Input;

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
