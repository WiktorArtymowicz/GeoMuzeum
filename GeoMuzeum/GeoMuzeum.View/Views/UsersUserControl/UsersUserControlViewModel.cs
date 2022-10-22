using GeoMuzeum.DataService;
using GeoMuzeum.Model;
using GeoMuzeum.View.ProjectHelpers;
using GeoMuzeum.View.Views.UsersUserControl.AddOrUpdateUser;
using GeoMuzeum.View.ViewServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GeoMuzeum.View.Views.UsersUserControl
{
    public class UsersUserControlViewModel : BaseViewModel
    {
        private readonly IUserDataService _userDataService;
        private readonly IUserLoginDataService _userLoginDataService;
        private readonly IUserLogDataService _userLogDataService;
        private readonly ICatalogDataService _catalogDataService;

        private User _sentUser;

        protected UsersUserControlViewModel()
        {

        }

        public UsersUserControlViewModel(IUserDataService userDataService, IUserLoginDataService userLoginDataService, IUserLogDataService userLogDataService, ICatalogDataService catalogDataService)
        {
            _userDataService = userDataService;
            _userLogDataService = userLogDataService;
            _userLoginDataService = userLoginDataService;
            _catalogDataService = catalogDataService;

            Users = new ObservableCollection<User>();
            UserSearchTypes = new List<UserSearchType>();
            UserSortTypes = new List<UserSortType>();
        }

        public override void SetUser(User user)
        {
            if (user == null)
            {
                MessageBox.Show("Błąd przy przekazywaniu użytkownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _sentUser = user;
        }

        public override async Task LoadDataAsync()
        {
            await LoadUsers();
            LoadSearchTypes();
            LoadSortTypes();
        }

        private void LoadSortTypes()
        {
            UserSortTypes.Clear();

            UserSortTypes = Enum.GetValues(typeof(UserSortType)).Cast<UserSortType>().ToList();
            SelectedUserSortType = UserSortTypes.FirstOrDefault();
        }

        private void LoadSearchTypes()
        {
            UserSearchTypes.Clear();

            UserSearchTypes = Enum.GetValues(typeof(UserSearchType)).Cast<UserSearchType>().ToList();
            SelectedUserSearchType = UserSearchTypes.FirstOrDefault();
        }

        private async Task LoadUsers()
        {
            Users.Clear();

            var userFromDb = await _userDataService.GetAllUsers();
            Users = userFromDb.Where(x => x.UserId != _sentUser.UserId).ToObservableCollection();

            SelectedUser = Users.FirstOrDefault();
        }

        private async void SearchUsersBy(string searchText)
        {
            if(string.IsNullOrWhiteSpace(searchText))
            {
                await LoadUsers();
                return;
            }

            if(SelectedUserSearchType == UserSearchType.Imię)
            {
                Users.Clear();

                var userFromDb = await _userDataService.GetUsersByName(searchText);
                Users = userFromDb.ToObservableCollection();

                SelectedUser = Users.FirstOrDefault();
            }

            if (SelectedUserSearchType == UserSearchType.Stanowisko)
            {
                Users.Clear();

                var userFromDb = await _userDataService.GetUsersByPosition(searchText);
                Users = userFromDb.ToObservableCollection();

                SelectedUser = Users.FirstOrDefault();
            }
        }

        private void SortUsersBy(UserSortType userSortType)
        {
            if (userSortType == UserSortType.Domyślnie)
                Users = Users.OrderBy(x => x.UserId).ToObservableCollection();

            if(userSortType == UserSortType.Imię)
                Users = Users.OrderBy(x => x.UserName).ToObservableCollection();

            if(userSortType == UserSortType.Stanowisko)
                Users = Users.OrderBy(x => x.UserPosition).ToObservableCollection();
        }

        private async void AddUser()
        {
            ViewMessenger.Default.Send<User>(null);

            var catalogViewService = new ViewDialogService<AddOrUpdateUserView>(new AddOrUpdateUserView(_sentUser));
            catalogViewService.ShowGenericWindow();

            await LoadDataAsync();
        }

        private async void UpdateUser()
        {
            if (SelectedUser == null)
                return;

            ViewMessenger.Default.Send(SelectedUser);

            var catalogViewService = new ViewDialogService<AddOrUpdateUserView>(new AddOrUpdateUserView(_sentUser));
            catalogViewService.ShowGenericWindow();

            await LoadDataAsync();
        }

        private async void DeleteUser()
        {
            if (SelectedUser == null)
                return;

            try
            {
                var question = MessageBox.Show($"Czy usunąć użytkownika {SelectedUser.UserName}?", "Pytanie", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (question == MessageBoxResult.No)
                    return;

                if (await _catalogDataService.AnyCatalogsByUser(SelectedUser))
                {
                    MessageBox.Show($"Nie można usunąć użytkownika {SelectedUser.UserName} ponieważ są do niego przypisane katalogi eksponatów.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (await _userLogDataService.AnyLogsByUser(SelectedUser))
                {
                    MessageBox.Show($"Ze względu na historyczność zapisanych logów, nie ma możliwości usunięcia użytkownika {SelectedUser.UserName}.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var userLogin = await _userLoginDataService.GetUserLoginByUserId(SelectedUser);

                await _userLoginDataService.DeleteUserLogin(userLogin);
                await _userDataService.DeleteUser(SelectedUser);

                await _userLogDataService.AddUserLog(new UserLog($"Użytkownik usunął użytkownika {SelectedUser.UserName}.", _sentUser));

                await LoadDataAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private ICommand _addUserCommand;
        public ICommand AddUserCommand
        {
            get
            {
                if (_addUserCommand == null)
                    _addUserCommand = new CustomCommand((object o) => { AddUser(); }, (object o) => { return true; });

                return _addUserCommand;
            }
            set { _addUserCommand = value; }
        }

        private ICommand _updateUserCommand;
        public ICommand UpdateUserCommand
        {
            get
            {
                if (_updateUserCommand == null)
                    _updateUserCommand = new CustomCommand((object o) => { UpdateUser(); }, (object o) => { return true; });

                return _updateUserCommand;
            }
            set { _updateUserCommand = value; }
        }

        private ICommand _deleteUserCommand;
        public ICommand DeleteUserCommand
        {
            get
            {
                if (_deleteUserCommand == null)
                    _deleteUserCommand = new CustomCommand((object o) => { DeleteUser(); }, (object o) => { return true; });

                return _deleteUserCommand;
            }
            set { _deleteUserCommand = value; }
        }

        private ICommand _refreschCommand;
        public ICommand RefreschCommand
        {
            get
            {
                if (_refreschCommand == null)
                    _refreschCommand = new CustomCommand(async (object o) => { await LoadDataAsync(); }, (object o) => { return true; });

                return _refreschCommand;
            }
            set { _refreschCommand = value; }
        }

        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get { return _users; }
            set { _users = value; OnPropertyChanged(nameof(Users)); }
        }

        private User _selectedUser;
        public User SelectedUser
        {
            get { return _selectedUser; }
            set { _selectedUser = value; OnPropertyChanged(nameof(SelectedUser)); }
        }

        private List<UserSearchType> _userSearchTypes;
        public List<UserSearchType> UserSearchTypes
        {
            get { return _userSearchTypes; }
            set { _userSearchTypes = value; OnPropertyChanged(nameof(UserSearchTypes)); }
        }

        private UserSearchType _selectedUserSearchType;
        public UserSearchType SelectedUserSearchType
        {
            get { return _selectedUserSearchType; }
            set { _selectedUserSearchType = value; OnPropertyChanged(nameof(SelectedUserSearchType)); }
        }

        private List<UserSortType> _userSortTypes;
        public List<UserSortType> UserSortTypes
        {
            get { return _userSortTypes; }
            set { _userSortTypes = value; OnPropertyChanged(nameof(UserSortTypes)); }
        }

        private UserSortType _selectedUserSortType;
        public UserSortType SelectedUserSortType
        {
            get { return _selectedUserSortType; }
            set
            {
                _selectedUserSortType = value;
                OnPropertyChanged(nameof(SelectedUserSortType));
                SortUsersBy(SelectedUserSortType);
            }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                SearchUsersBy(SearchText);
            }
        }
    }
}
