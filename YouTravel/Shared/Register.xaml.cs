using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YouTravel.Model;

namespace YouTravel.Shared
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private string _username = "";
        public string Username
        {
            get { return _username; }
            set { _username = value; DoPropertyChanged(nameof(Username)); }
        }

        private string _name = "";
        // Can't use Name since it hides an inherited member
        public string UName
        {
            get { return _name; }
            set { _name = value; DoPropertyChanged(nameof(UName)); }
        }

        private string _surname = "";
        public string Surname
        {
            get { return _surname; }
            set { _surname = value; DoPropertyChanged(nameof(Surname)); }
        }

        private string _email = "";
        public string Email
        {
            get { return _email; }
            set { _email = value; DoPropertyChanged(nameof(Email)); }
        }

        private string _password = "";
        public string Password
        {
            get { return _password; }
            set { _password = value; DoPropertyChanged(nameof(Password)); }
        }

        private bool _usernameError = true;
        public bool UsernameError
        {
            get { return _usernameError; }
            set { _usernameError = value; ValidateForm(); DoPropertyChanged(nameof(UsernameError)); }
        }

        private bool _nameError = true;
        public bool NameError
        {
            get { return _nameError; }
            set { _nameError = value; ValidateForm(); DoPropertyChanged(nameof(NameError)); }
        }

        private bool _surnameError = true;
        public bool SurnameError
        {
            get { return _surnameError; }
            set { _surnameError = value; ValidateForm(); DoPropertyChanged(nameof(SurnameError)); }
        }

        private bool _emailError = true;
        public bool EmailError
        {
            get { return _emailError; }
            set { _emailError = value; ValidateForm(); DoPropertyChanged(nameof(EmailError)); }
        }

        private bool _passwordError = true;
        public bool PasswordError
        {
            get { return _passwordError; }
            set { _passwordError = value; ValidateForm(); DoPropertyChanged(nameof(PasswordError)); }
        }

        public new User? DialogResult { get; set; }

        private bool _validForm = false;
        public bool ValidForm
        {
            get { return _validForm; }
            set { _validForm = value; DoPropertyChanged(nameof(ValidForm)); }
        }

        public Register()
        {
            InitializeComponent();
            DataContext = this;

            Owner = Application.Current.MainWindow;
            tbUsername.Focus();
        }

        private void ValidateForm()
        {
            ValidForm = !(UsernameError || EmailError || PasswordError || NameError || SurnameError);
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            AttemptRegister();
        }

        private void AttemptRegister()
        {
            if (!ValidForm) return;
            using var db = new TravelContext();
            var user = new User()
            {
                Username = Username,
                Name = UName,
                Surname = Surname,
                Email = Email,
                Password = Password,
            };
            db.Users.Add(user);
            db.SaveChanges();
            DialogResult = user;
            Close();
        }

        private void TbUsername_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AttemptRegister();
            }
            UsernameError = IsInvalidField(Username);
        }

        private void TbName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AttemptRegister();
            }
            NameError = IsInvalidField(Name);
        }

        private void TbSurname_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AttemptRegister();
            }
            SurnameError = IsInvalidField(Surname);
        }

        private void TbEmail_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AttemptRegister();
            }
            EmailError = IsInvalidField(Email);
        }

        private void TbPassword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AttemptRegister();
            }
            PasswordError = IsInvalidField(Password);
        }

        private static bool IsInvalidField(string field)
        {
            return field == "";
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void TbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = ((PasswordBox)sender).Password;
        }
    }
}
