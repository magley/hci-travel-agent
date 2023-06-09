using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using YouTravel.Util;

namespace YouTravel.Shared
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private string _username = "";
        public string Username
        {
            get { return _username; }
            set { _username = value; DoPropertyChanged(nameof(Username)); }
        }

        private string _password = "";
        public string Password
        { 
            get { return _password; }
            set { _password = value; DoPropertyChanged(nameof(Password)); }
        }

        private string? _errorText;
        public string? ErrorText
        { 
            get { return _errorText; }
            set { _errorText = value; DoPropertyChanged(nameof(ErrorText)); }
        }

        public Login()
        {
            InitializeComponent();
            DataContext = this;

            // TODO: Is this necessary if we call the window with ShowDialog() instead of Show()?
            Owner = Application.Current.MainWindow;
            tbUsername.Focus();
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            AttemptLogin();
        }

        private void AttemptLogin()
        {
            try
            {
                YouTravelContext.Login(Username, Password);
                // TODO: Play a sound or something fancy
                Close();
            }
            catch (LoginFailedException)
            {
                ErrorText = "Wrong username or password";
            }
        }

        private void TbUsername_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AttemptLogin();
            } else
            {
                ErrorText = null;
            }
        }

        private void TbPassword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AttemptLogin();
            } else
            {
                ErrorText = null;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
