using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YouTravel.Model;
using YouTravel.Util;

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

        private bool _usernameEmptyError = true;
        public bool UsernameEmptyError
        {
            get { return _usernameEmptyError; }
            set { _usernameEmptyError = value; ValidateForm(); DoPropertyChanged(nameof(UsernameEmptyError)); }
        }

        private bool _usernameTakenError = false;
        public bool UsernameTakenError
        {
            get { return _usernameTakenError; }
            set { _usernameTakenError = value; ValidateForm(); DoPropertyChanged(nameof(UsernameTakenError)); }
        }

        private bool _nameEmptyError = true;
        public bool NameEmptyError
        {
            get { return _nameEmptyError; }
            set { _nameEmptyError = value; ValidateForm(); DoPropertyChanged(nameof(NameEmptyError)); }
        }

        private bool _surnameEmptyError = true;
        public bool SurnameEmptyError
        {
            get { return _surnameEmptyError; }
            set { _surnameEmptyError = value; ValidateForm(); DoPropertyChanged(nameof(SurnameEmptyError)); }
        }

        private bool _emailEmptyError = true;
        public bool EmailEmptyError
        {
            get { return _emailEmptyError; }
            set { _emailEmptyError = value; ValidateForm(); DoPropertyChanged(nameof(EmailEmptyError)); }
        }

        private bool _passwordEmptyError = true;
        public bool PasswordEmptyError
        {
            get { return _passwordEmptyError; }
            set { _passwordEmptyError = value; ValidateForm(); DoPropertyChanged(nameof(PasswordEmptyError)); }
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
            ValidForm = !(UsernameEmptyError || UsernameTakenError || EmailEmptyError || PasswordEmptyError || NameEmptyError || SurnameEmptyError);
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
            try
            {
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
            catch (DbUpdateException e)
            {
                var inner = (SqliteException?)e.InnerException ?? throw e;
                if (inner.SqliteErrorCode == SQLitePCL.raw.SQLITE_CONSTRAINT)
                {
                    UsernameTakenError = true;
                    UsernameEmptyError = false;
                }
                else
                {
                    throw e;
                }
            }
        }

        private void TbUsername_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AttemptRegister();
            }
            UsernameEmptyError = IsInvalidField(Username);
            UsernameTakenError = false;
        }

        private void TbName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AttemptRegister();
            }
            NameEmptyError = IsInvalidField(Name);
        }

        private void TbSurname_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AttemptRegister();
            }
            SurnameEmptyError = IsInvalidField(Surname);
        }

        private void TbEmail_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AttemptRegister();
            }
            EmailEmptyError = IsInvalidField(Email);
        }

        private void TbPassword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AttemptRegister();
            }
            PasswordEmptyError = IsInvalidField(Password);
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

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IInputElement focusedControl = FocusManager.GetFocusedElement(Application.Current.Windows[1]);
            if (focusedControl is DependencyObject depObject)
            {
                string str = HelpProvider.GetHelpKey(depObject);
                HelpProvider.ShowHelp(str, this);
            }
            else
            {
                if (YouTravelContext.User?.Type == UserType.AGENT)
                {
                    HelpProvider.ShowHelp("index_agent", this);
                }
                else
                {
                    HelpProvider.ShowHelp("index_client", this);
                }
            }
        }
    }
}
