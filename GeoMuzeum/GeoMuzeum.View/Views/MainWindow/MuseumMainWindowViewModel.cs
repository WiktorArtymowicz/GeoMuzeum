using GeoMuzeum.Model;
using GeoMuzeum.View.Enums;
using GeoMuzeum.View.ProjectHelpers;
using GeoMuzeum.View.Views.LoginView;
using GeoMuzeum.View.ViewServices;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GeoMuzeum.View.Views.MainWindow
{
    public class MuseumMainWindowViewModel : BaseViewModel
    {
        private User _sentUser;

        public Action CloseAction { get; set; }

        public MuseumMainWindowViewModel()
        { 

        }

        public override void SetUser(User user)
        {
            if (user == null)
            {
                MessageBox.Show("Użytkownik nie został zalogowany.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                CloseAction();
            }

            _sentUser = user;
            UserInfo = $"Użytkownik: {_sentUser.UserName} - {_sentUser.UserPositionInfo}";
        }

        private bool AdminPermissions()
        {
            if (_sentUser == null)
                return false;

            return _sentUser.UserPosition == Model.Enums.UserPosition.Admin;
        }

        private bool ManagerPermissions()
        {
            if (_sentUser == null)
                return false;

            return _sentUser.UserPosition == Model.Enums.UserPosition.Admin || _sentUser.UserPosition == Model.Enums.UserPosition.Kierownik;
        }

        private async Task ChangeViewModel(ViewModelType type)
        {
            try
            {
                if (type == ViewModelType.CatalogsUserControlViewModel)
                {
                    SelectedViewModel = ViewModelLocator.CatalogsUserControlViewModel;
                    SelectedViewModel.SetUser(_sentUser);
                    await SelectedViewModel.LoadDataAsync();
                }

                if (type == ViewModelType.ExhibitsUserControlViewModel)
                {
                    SelectedViewModel = ViewModelLocator.ExhibitsUserControlViewModel;
                    SelectedViewModel.SetUser(_sentUser);
                    await SelectedViewModel.LoadDataAsync();
                }

                if (type == ViewModelType.ExhibitsLocalizationsUserControlViewModel)
                {
                    SelectedViewModel = ViewModelLocator.ExhibitsLocalizationsUserControlViewModel;
                    SelectedViewModel.SetUser(_sentUser);
                    await SelectedViewModel.LoadDataAsync();
                }

                if (type == ViewModelType.ExhibitsStocktakingUserControlViewModel)
                {
                    SelectedViewModel = ViewModelLocator.ExhibitsStocktakingUserControlViewModel;
                    SelectedViewModel.SetUser(_sentUser);
                    await SelectedViewModel.LoadDataAsync();
                }

                if (type == ViewModelType.ToolsUserControlViewModel)
                {
                    SelectedViewModel = ViewModelLocator.ToolsUserControlViewModel;
                    SelectedViewModel.SetUser(_sentUser);
                    await SelectedViewModel.LoadDataAsync();
                }

                if (type == ViewModelType.ToolsLocalizationsUserControlViewModel)
                {
                    SelectedViewModel = ViewModelLocator.ToolsLocalizationsUserControlViewModel;
                    SelectedViewModel.SetUser(_sentUser);
                    await SelectedViewModel.LoadDataAsync();
                }

                if (type == ViewModelType.UsersUserControlViewModel)
                {
                    SelectedViewModel = ViewModelLocator.UsersUserControlViewModel;
                    SelectedViewModel.SetUser(_sentUser);
                    await SelectedViewModel.LoadDataAsync();
                }

                if (type == ViewModelType.UsersLogUserControlViewModel)
                {
                    SelectedViewModel = ViewModelLocator.UsersLogUserControlViewModel;
                    SelectedViewModel.SetUser(_sentUser);
                    await SelectedViewModel.LoadDataAsync();
                }

                if (type == ViewModelType.ToolsStocktanikgUserControlViewModel)
                {
                    SelectedViewModel = ViewModelLocator.ToolsStocktakingUserControlViewModel;
                    SelectedViewModel.SetUser(_sentUser);
                    await SelectedViewModel.LoadDataAsync();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        public override async Task LoadDataAsync()
        {
            await ChangeViewModel(ViewModelType.CatalogsUserControlViewModel);
        }

        private void CloseWindow()
        {
            var question = MessageBox.Show("Czy na pewno wyjść z aplikacji?", "Pytanie", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (question == MessageBoxResult.No)
                return;

            CloseAction();
        }

        private void Logout()
        {
            var question = MessageBox.Show("Czy na pewno wylogować użytkownika z aplikacji?", "Pytanie", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (question == MessageBoxResult.No)
                return;

            var loginView = new UserLoginView();
            loginView.Show();

            CloseAction();
        }


        private ICommand _selectExhibitViewModelCommand;

        public ICommand SelectExhibitViewModelCommand
        {
            get
            {
                if (_selectExhibitViewModelCommand == null)
                    _selectExhibitViewModelCommand = new CustomCommand(async (object o) => { await ChangeViewModel(ViewModelType.ExhibitsUserControlViewModel); }, (object o) => { return true; });

                return _selectExhibitViewModelCommand;
            }
            set { _selectExhibitViewModelCommand = value; }
        }

        private ICommand _selectCatalogsViewModelCommand;

        public ICommand SelectCatalogViewModelCommand
        {
            get
            {
                if (_selectCatalogsViewModelCommand == null)
                    _selectCatalogsViewModelCommand = new CustomCommand(async (object o) => { await ChangeViewModel(ViewModelType.CatalogsUserControlViewModel); }, (object o) => { return true; });

                return _selectCatalogsViewModelCommand;
            }
            set { _selectCatalogsViewModelCommand = value; }
        }

        private ICommand _selectExhibistLocationsViewModelCommand;

        public ICommand SelectExhibistLocationsViewModelCommand
        {
            get
            {
                if (_selectExhibistLocationsViewModelCommand == null)
                    _selectExhibistLocationsViewModelCommand = new CustomCommand(async (object o) => { await ChangeViewModel(ViewModelType.ExhibitsLocalizationsUserControlViewModel); }, (object o) => { return true; });

                return _selectExhibistLocationsViewModelCommand;
            }
            set { _selectExhibistLocationsViewModelCommand = value; }
        }

        private ICommand _selectExhibitsStocktakingUserControlCommand;

        public ICommand SelectExhibitsStocktakingUserControlCommand
        {
            get
            {
                if (_selectExhibitsStocktakingUserControlCommand == null)
                    _selectExhibitsStocktakingUserControlCommand = new CustomCommand(async (object o) => { await ChangeViewModel(ViewModelType.ExhibitsStocktakingUserControlViewModel); }, (object o) => ManagerPermissions());

                return _selectExhibitsStocktakingUserControlCommand;
            }
            set { _selectExhibitsStocktakingUserControlCommand = value; }
        }

        private ICommand _selectToolsUserControlViewCommand;

        public ICommand SelectToolsUserControlViewCommand
        {
            get
            {
                if (_selectToolsUserControlViewCommand == null)
                    _selectToolsUserControlViewCommand = new CustomCommand(async (object o) => { await ChangeViewModel(ViewModelType.ToolsUserControlViewModel); }, (object o) => { return true; });

                return _selectToolsUserControlViewCommand;
            }
            set { _selectToolsUserControlViewCommand = value; }
        }

        private ICommand _selectToolsLocalizationsUserControlViewCommand;

        public ICommand SelectToolsLocalizationsUserControlViewCommand
        {
            get 
            {
                if (_selectToolsLocalizationsUserControlViewCommand == null)
                    _selectToolsLocalizationsUserControlViewCommand = new CustomCommand(async (object o) => { await ChangeViewModel(ViewModelType.ToolsLocalizationsUserControlViewModel); }, (object o) => { return true; });

                return _selectToolsLocalizationsUserControlViewCommand; 
            }
            set { _selectToolsLocalizationsUserControlViewCommand = value; }
        }


        private ICommand _selectUsersUserControlViewCommand;

        public ICommand SelectUsersUserControlViewCommand
        {
            get 
            {
                if (_selectUsersUserControlViewCommand == null)
                    _selectUsersUserControlViewCommand = new CustomCommand(async (object o) => { await ChangeViewModel(ViewModelType.UsersUserControlViewModel); }, (object o) => AdminPermissions()); 

                return _selectUsersUserControlViewCommand; 
            }
            set { _selectUsersUserControlViewCommand = value; }
        }

        private ICommand _selectUsersLogUserConctolViewCommand;

        public ICommand SelectUsersLogUserControlViewCommand
        {
            get 
            {
                if (_selectUsersLogUserConctolViewCommand == null)
                    _selectUsersLogUserConctolViewCommand = new CustomCommand(async (object o) => { await ChangeViewModel(ViewModelType.UsersLogUserControlViewModel); }, (object o) => ManagerPermissions());

                return _selectUsersLogUserConctolViewCommand; 
            }
            set { _selectUsersLogUserConctolViewCommand = value; }
        }

        private ICommand _selectToolsStocktakingUserControlViewCommand;

        public ICommand SelectToolsStocktakingUserControlViewCommand
        {
            get 
            {
                if (_selectToolsStocktakingUserControlViewCommand == null)
                    _selectToolsStocktakingUserControlViewCommand = new CustomCommand(async (object o) => { await ChangeViewModel(ViewModelType.ToolsStocktanikgUserControlViewModel); }, (object o) => ManagerPermissions());

                return _selectToolsStocktakingUserControlViewCommand; 
            }
            set { _selectToolsStocktakingUserControlViewCommand = value; }
        }

        private ICommand _closeWindowCommand;

        public ICommand CloseWindowCommand
        {
            get
            {
                if (_closeWindowCommand == null)
                    _closeWindowCommand = new CustomCommand((object o) => { CloseWindow(); }, (object o) => { return true; });

                return _closeWindowCommand;
            }
            set { _closeWindowCommand = value; }
        }

        private ICommand _logoutCommand;

        public ICommand LogoutCommand
        {
            get
            {
                if (_logoutCommand == null)
                    _logoutCommand = new CustomCommand((object o) => { Logout(); }, (object o) => { return true; });

                return _logoutCommand;
            }
            set { _logoutCommand = value; }
        }

      
        private BaseViewModel _selectedViewModel;

        public BaseViewModel SelectedViewModel
        {
            get { return _selectedViewModel; }
            set { _selectedViewModel = value; OnPropertyChanged(nameof(SelectedViewModel)); }
        }

        private string _userInfo;

        public string UserInfo
        {
            get { return _userInfo; }
            set { _userInfo = value; OnPropertyChanged(nameof(UserInfo)); }
        }
    }
}
