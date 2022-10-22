using GeoMuzeum.DataService;
using GeoMuzeum.Model;
using GeoMuzeum.View.ProjectHelpers;
using GeoMuzeum.View.ViewServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GeoMuzeum.View.Views.UsersLogUserControl
{
    public class UsersLogUserControlViewModel : BaseViewModel
    {
        private readonly IUserLogDataService _userLogDataService;

        protected UsersLogUserControlViewModel()
        {

        }

        public UsersLogUserControlViewModel(IUserLogDataService userLogDataService)
        {
            _userLogDataService = userLogDataService;

            UserLogs = new ObservableCollection<UserLog>();
            UsersLogSearchTypes = new List<UsersLogSearchType>();
            UsersLogSortTypes = new List<UsersLogSortType>();
        }

        public override async Task LoadDataAsync()
        {
            try
            {
                await LoadUserLogs();
                LoadSortTypes();
                LoadSearchTypes();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void LoadSearchTypes()
        {
            UsersLogSearchTypes.Clear();

            UsersLogSearchTypes = Enum.GetValues(typeof(UsersLogSearchType)).Cast<UsersLogSearchType>().ToList();
            SelectedUserLogSearchType = UsersLogSearchTypes.FirstOrDefault();
        }

        private void LoadSortTypes()
        {
            UsersLogSortTypes.Clear();

            UsersLogSortTypes = Enum.GetValues(typeof(UsersLogSortType)).Cast<UsersLogSortType>().ToList();
            SelectedUserLogSortType = UsersLogSortTypes.FirstOrDefault();
        }

        private async Task LoadUserLogs()
        {
            UserLogs.Clear();

            var userLogs = await _userLogDataService.GetAllUserLogs();
            UserLogs = userLogs.ToObservableCollection();

            SelectedUserLog = UserLogs.FirstOrDefault();
        }

        private async void SearchLogsBy(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                await LoadUserLogs();
                return;
            }

            if (SelectedUserLogSearchType == UsersLogSearchType.Użytkownik)
            {
                var userLogs = await _userLogDataService.GetUserLogsByUserName(searchText);
                UserLogs = userLogs.ToObservableCollection();
            }

            if (SelectedUserLogSearchType == UsersLogSearchType.Opis)
            {
                var userLogs = await _userLogDataService.GetUserLogsByDescription(searchText);
                UserLogs = userLogs.ToObservableCollection();
            }
        }

        private void SortLogsBy(UsersLogSortType userLogSortType)
        {
            if (userLogSortType == UsersLogSortType.Domyślnie)
                UserLogs = UserLogs.OrderBy(x => x.UserLogId).ToObservableCollection();

            if (userLogSortType == UsersLogSortType.Opis)
                UserLogs = UserLogs.OrderBy(x => x.LogDescription).ToObservableCollection();

            if (userLogSortType == UsersLogSortType.Użytkownik)
                UserLogs = UserLogs.OrderBy(x => x.User.UserName).ToObservableCollection();
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

        private ObservableCollection<UserLog> _userLogs;
        public ObservableCollection<UserLog> UserLogs
        {
            get { return _userLogs; }
            set { _userLogs = value; OnPropertyChanged(nameof(UserLogs)); }
        }

        private UserLog _selectedUserLog;
        public UserLog SelectedUserLog
        {
            get { return _selectedUserLog; }
            set { _selectedUserLog = value; OnPropertyChanged(nameof(SelectedUserLog)); }
        }

        private List<UsersLogSearchType> _usersLogSearchTypes;
        public List<UsersLogSearchType> UsersLogSearchTypes
        {
            get { return _usersLogSearchTypes; }
            set { _usersLogSearchTypes = value; OnPropertyChanged(nameof(UsersLogSearchTypes)); }
        }

        private UsersLogSearchType _selectedUserLogSearchType;
        public UsersLogSearchType SelectedUserLogSearchType
        {
            get { return _selectedUserLogSearchType; }
            set { _selectedUserLogSearchType = value; OnPropertyChanged(nameof(SelectedUserLogSearchType)); }
        }

        private List<UsersLogSortType> _usersLogSortTypes;
        public List<UsersLogSortType> UsersLogSortTypes
        {
            get { return _usersLogSortTypes; }
            set { _usersLogSortTypes = value; OnPropertyChanged(nameof(UsersLogSortTypes)); }
        }

        private UsersLogSortType _selectedUserLogSortType;
        public UsersLogSortType SelectedUserLogSortType
        {
            get { return _selectedUserLogSortType; }
            set
            {
                _selectedUserLogSortType = value;
                OnPropertyChanged(nameof(SelectedUserLogSortType));
                SortLogsBy(SelectedUserLogSortType);
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
                SearchLogsBy(SearchText);
            }
        }
    }
}
