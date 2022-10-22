using GeoMuzeum.DataService;
using GeoMuzeum.Model;
using GeoMuzeum.View.ProjectHelpers;
using GeoMuzeum.View.Views.MainWindow;
using GeoMuzeum.View.ViewServices;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GeoMuzeum.View.Views.LoginView
{
    public class UserLoginViewModel : BaseViewModel
    {
        private readonly IUserLoginDataService _userLoginDataService;

        public Action CloseAction { get; set; }

        protected UserLoginViewModel()
        {

        }

        public UserLoginViewModel(IUserLoginDataService userLoginDataService)
        {
            _userLoginDataService = userLoginDataService;
        }

        public void CleanData()
        {
            UserLogin = string.Empty;
            UserPassword = string.Empty;
        }

        private void CloseWindow()
        {
            CloseAction();
        }

        private async void LoginUser()
        {
            try
            {
                if(string.IsNullOrWhiteSpace(UserLogin))
                {
                    MessageBox.Show("Proszę wprowadzić nazwę użytkownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var userLogin = await _userLoginDataService.TryFoundUserByLogin(UserLogin);

                if(userLogin == null)
                {
                    MessageBox.Show("Nie znaleziono użytkownika o podanym loginie.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if(string.IsNullOrWhiteSpace(UserPassword))
                {
                    MessageBox.Show("Proszę wprowadzić numer PIN użytkownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if(int.TryParse(UserPassword, out int result))
                {
                    if (userLogin.PinNumber.Equals(result))
                    {
                        FoundUser = userLogin.User;
                    }
                    else
                    {
                        MessageBox.Show("Ponady PIN jest nieprawidłowy.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Podana wartość nie jest wartością liczbową.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var mainView = new MuseumMainWindow(ViewModelLocator.MuseumMainWindowViewModel);
                var dataContext = mainView.DataContext as MuseumMainWindowViewModel;
                dataContext.SetUser(FoundUser);
                mainView.Show();
                
                CloseAction();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private ICommand _loginCommand;
        public ICommand LoginCommand
        {
            get
            {
                if (_loginCommand == null)
                    _loginCommand = new CustomCommand((object o) => { LoginUser(); }, (object o) => { return true; });

                return _loginCommand;
            }
            set { _loginCommand = value; }
        }

        
        private ICommand _closeCommand;
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                    _closeCommand = new CustomCommand((object o) => { CloseWindow(); }, (object o) => { return true; });

                return _closeCommand;
            }
            set { _closeCommand = value; }
        }

        private User _foundUser;
        public User FoundUser
        {
            get { return _foundUser; }
            set { _foundUser = value; OnPropertyChanged(nameof(FoundUser)); }
        }

        private string _userLogin;
        public string UserLogin
        {
            get { return _userLogin; }
            set { _userLogin = value; OnPropertyChanged(nameof(UserLogin)); }
        }

        private string _userPassword;
        public string UserPassword
        {
            get { return _userPassword; }
            set { _userPassword = value; OnPropertyChanged(nameof(UserPassword)); }
        }
    }
}
