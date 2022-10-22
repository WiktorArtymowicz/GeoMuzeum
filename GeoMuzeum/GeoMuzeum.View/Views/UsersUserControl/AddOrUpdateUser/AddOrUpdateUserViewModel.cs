using GeoMuzeum.DataService;
using GeoMuzeum.Model;
using GeoMuzeum.Model.Enums;
using GeoMuzeum.View.Enums;
using GeoMuzeum.View.ProjectHelpers;
using GeoMuzeum.View.ViewServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GeoMuzeum.View.Views.UsersUserControl.AddOrUpdateUser
{
    public class AddOrUpdateUserViewModel : BaseViewModel
    {
        private readonly IUserDataService _userDataService;
        private readonly IUserLoginDataService _userLoginDataService;
        private readonly IUserLogDataService _userLogDataService;

        private EditStatusType _editStatusType;
        private User _mainUser;

        public Action CloseAction { get; set; }

        protected AddOrUpdateUserViewModel()
        {

        }

        public AddOrUpdateUserViewModel(IUserDataService userDataService, IUserLoginDataService userLoginDataService, IUserLogDataService userLogDataService)
        {
            _userDataService = userDataService;
            _userLoginDataService = userLoginDataService;
            _userLogDataService = userLogDataService;

            UserPositions = new List<UserPosition>();

            ViewMessenger.Default.Register<User>(this, OnUserRecived);
        }

        public async override Task LoadDataAsync()
        {
            UserPositions = Enum.GetValues(typeof(UserPosition)).Cast<UserPosition>().ToList();

            if (_editStatusType == EditStatusType.Add)
            {
                UserLogin = new UserLogin();

                UserName = string.Empty;
                UserSurname = string.Empty;
                Login = string.Empty;
                UserPin = string.Empty;

                SelectedUserPosition = UserPositions.FirstOrDefault();
            }
            else
            {
                UserLogin = await _userLoginDataService.GetUserLoginByUserId(SentUser);

                UserName = UserLogin.User.UserName;
                UserSurname = UserLogin.User.UserSurname;
                Login = UserLogin.Login;
                UserPin = UserLogin.PinNumber.ToString();

                SelectedUserPosition = UserPositions.FirstOrDefault(x => x == UserLogin.User.UserPosition);
            }
        }

        private async void OnUserRecived(User user)
        {
            SentUser = user == null ? new User() : user;
            _editStatusType = user == null ? EditStatusType.Add : EditStatusType.Modify;
        }

        public override void SetUser(User user)
        {
            if (user == null)
            {
                MessageBox.Show("Błąd przy przekazywaniu użytkownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                CloseAction();
            }

            _mainUser = user;
        }

        private async Task<bool> ValidateDataToSave()
        {
            if (string.IsNullOrWhiteSpace(UserName))
            {
                MessageBox.Show($"Proszę uzupełnić imię użytkownika.", "Brak wprowadzonych danych.", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(UserSurname))
            {
                MessageBox.Show($"Proszę uzupełnić nazwisko użytkownika.", "Brak wprowadzonych danych.", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Login))
            {
                MessageBox.Show($"Proszę uzupełnić login użytkownika.", "Brak wprowadzonych danych.", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if(await _userLoginDataService.CheckUserLogin(Login) && _editStatusType == EditStatusType.Add)
            {
                MessageBox.Show($"Podany login już istnieje w bazie danych.", "Brak wprowadzonych danych.", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(UserPin))
            {
                MessageBox.Show($"Proszę uzupełnić numer Pin użytkownika.", "Brak wprowadzonych danych.", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (UserPin.Length < 4)
            {
                MessageBox.Show($"Pin powinien posiadać 4 cyfry.", "Brak wprowadzonych danych.", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (UserPin.IsNotInt())
            {
                MessageBox.Show($"Wprowadzona wartość nie jest liczbą.", "Brak wprowadzonych danych.", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (await _userLoginDataService.CheckUserPin(UserPin.StringToInt()) && _editStatusType == EditStatusType.Add)
            {
                MessageBox.Show($"Wprowadzony pin istnieje.", "Brak wprowadzonych danych.", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private UserLogin FillUserLoginToSave(User user)
        {
            UserLogin.Login = Login;
            UserLogin.PinNumber = int.Parse(UserPin);
            UserLogin.User = user;

            return UserLogin;
        }

        private User FillUserToSave()
        {
            SentUser.UserName = UserName;
            SentUser.UserSurname = UserSurname;
            SentUser.UserPosition = SelectedUserPosition;

            return SentUser;
        }

        private async void AddOrUpdateUser()
        {
            try
            {
                if (await ValidateDataToSave() == false)
                    return;

                var userToSave = FillUserToSave();
                var userLoginToSave = FillUserLoginToSave(userToSave);

                if (_editStatusType == EditStatusType.Add)
                {
                    await _userDataService.AddNewUser(userToSave);
                    await _userLoginDataService.AddUserLogin(userLoginToSave);

                    await _userLogDataService.AddUserLog(new UserLog($"Użytkownik dodał nowego użytkownika {userToSave.UserName}.", _mainUser));
                }
                else
                {
                    await _userDataService.UpdateUser(userToSave);
                    await _userLoginDataService.UpdateUserLogin(userLoginToSave);

                    await _userLogDataService.AddUserLog(new UserLog($"Użytkownik edytował użytkownika {userToSave.UserName}.", _mainUser));
                }

                CloseWindow();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Brak wprowadzonych danych.", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void CloseWindow()
        {
            CloseAction();
        }

        private async void GenerateUserPin()
        {
            try
            {
                var question = MessageBox.Show("Czy wygenerować numer pin?", "Pytanie", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (question == MessageBoxResult.No)
                    return;

                UserPin = string.Empty;
                var pin = await GeneratePin.Generate();

                MessageBox.Show($"Wygenerowano PIN: {pin}", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);

                UserPin = pin.ToString();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
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

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                    _saveCommand = new CustomCommand((object o) => { AddOrUpdateUser(); }, (object o) => { return true; });

                return _saveCommand;
            }
            set { _saveCommand = value; }
        }

        private ICommand _generatePinCommand;
        public ICommand GeneratePinCommand
        {
            get
            {
                if (_generatePinCommand == null)
                    _generatePinCommand = new CustomCommand((object o) => { GenerateUserPin(); }, (object o) => { return true; });

                return _generatePinCommand;
            }
            set { _generatePinCommand = value; }
        }

        private UserLogin _userLogin;
        public UserLogin UserLogin
        {
            get { return _userLogin; }
            set { _userLogin = value; OnPropertyChanged(nameof(UserLogin)); }
        }


        private User _sentUser;
        public User SentUser
        {
            get { return _sentUser; }
            set { _sentUser = value; OnPropertyChanged(nameof(SentUser)); }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; OnPropertyChanged(nameof(UserName)); }
        }

        private string _userSurname;
        public string UserSurname
        {
            get { return _userSurname; }
            set { _userSurname = value; OnPropertyChanged(nameof(UserSurname)); }
        }

        private string _login;
        public string Login
        {
            get { return _login; }
            set { _login = value; OnPropertyChanged(nameof(Login)); }
        }

        private string _userPin;
        public string UserPin
        {
            get { return _userPin; }
            set { _userPin = value; OnPropertyChanged(nameof(UserPin)); }
        }

        private List<UserPosition> _userPositions;
        public List<UserPosition> UserPositions
        {
            get { return _userPositions; }
            set { _userPositions = value; OnPropertyChanged(nameof(UserPositions)); }
        }

        private UserPosition _selectedUserPosition;
        public UserPosition SelectedUserPosition
        {
            get { return _selectedUserPosition; }
            set { _selectedUserPosition = value; OnPropertyChanged(nameof(SelectedUserPosition)); }
        }
    }
}
