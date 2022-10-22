using GeoMuzeum.DataService;
using GeoMuzeum.View.Views.CatalogsUserControl;
using GeoMuzeum.View.Views.CatalogsUserControl.AddOrUpdateCatalogWindow;
using GeoMuzeum.View.Views.ExhibitsLocalizationsUserControl;
using GeoMuzeum.View.Views.ExhibitsLocalizationsUserControl.AddOrUpdateExhibitLocalization;
using GeoMuzeum.View.Views.ExhibitsStocktakingUserControl;
using GeoMuzeum.View.Views.ExhibitsUserControl;
using GeoMuzeum.View.Views.ExhibitsUserControl.AddOrUpdateExhibit;
using GeoMuzeum.View.Views.MainWindow;
using GeoMuzeum.View.Views.ToolsLocalizationsUserControl;
using GeoMuzeum.View.Views.ToolsLocalizationsUserControl.AddOrUpdateToolsLocalization;
using GeoMuzeum.View.Views.ToolsStocktakingUserControl;
using GeoMuzeum.View.Views.ToolsUserControl;
using GeoMuzeum.View.Views.ToolsUserControl.AddOrUpdateTool;
using GeoMuzeum.View.Views.LoginView;
using GeoMuzeum.View.Views.UsersLogUserControl;
using GeoMuzeum.View.Views.UsersUserControl;
using GeoMuzeum.View.Views.UsersUserControl.AddOrUpdateUser;
using GeoMuzeum.View.Views.ExhibitsStocktakingUserControl.AddOrUpdateExhibitStocktaking;
using GeoMuzeum.View.Views.ToolsStocktakingUserControl.AddOrUpdateToolStocktaking;

namespace GeoMuzeum.View.ViewServices
{
    public class ViewModelLocator
    {
        private static ICatalogDataService _catalogDataService = new CatalogDataService();
        private static IExhibitDataService _exhibitDataService = new ExhibitDataService();
        private static IExhibitLocalizationDataService _exhibitLocalizationDataService = new ExhibitLocalizationDataService();
        private static IExhibitStocktakingDataService _exhibitStocktakingDataService = new ExhibitStocktakingDataService();
        private static IToolLocalizationDataService _toolLocalizationDataService = new ToolLocalizationDataService();
        private static IToolDataService _toolDataService = new ToolDataService();
        private static IUserDataService _userDataService = new UserDataService();
        private static IUserLogDataService _userLogDataService = new UserLogDataService();
        private static IToolStocktakingDataService _toolStocktakingDataService = new ToolStocktakingDataService();
        private static IUserLoginDataService _userLoginDataService = new UserLoginDataService();
        private static ISettingsDataService _settingsDataService = new SettingsDataService();

        private static MuseumMainWindowViewModel _museumMainWindowViewModel = new MuseumMainWindowViewModel();
        private static ExhibitsUserControlViewModel _exhibitsUserControlViewModel = new ExhibitsUserControlViewModel(_exhibitDataService, _userLogDataService, _settingsDataService);
        private static CatalogsUserControlViewModel _catalogsUserControlViewModel = new CatalogsUserControlViewModel(_catalogDataService, _userLogDataService);
        private static ExhibitsLocalizationsUserControlViewModel _exhibitsLocalizationsUserControlViewModel = new ExhibitsLocalizationsUserControlViewModel(_exhibitLocalizationDataService, _exhibitDataService, _userLogDataService);
        private static ExhibitsStocktakingUserControlViewModel _exhibitsStocktakingUserControlViewModel = new ExhibitsStocktakingUserControlViewModel(_exhibitStocktakingDataService, _userLogDataService, _settingsDataService);
        private static ToolsUserControlViewModel _toolsUserControlViewModel = new ToolsUserControlViewModel(_toolDataService, _userLogDataService, _settingsDataService);
        private static ToolsLocalizationsUserControlViewModel _toolsLocalizationsUserControlViewModel = new ToolsLocalizationsUserControlViewModel(_toolLocalizationDataService, _userLogDataService);
        private static UsersUserControlViewModel _usersUserControlViewModel = new UsersUserControlViewModel(_userDataService,_userLoginDataService, _userLogDataService, _catalogDataService);
        private static UsersLogUserControlViewModel _usersLogUserControlViewModel = new UsersLogUserControlViewModel(_userLogDataService);
        private static ToolsStocktakingUserControlViewModel _toolsStocktakingUserControlViewModel = new ToolsStocktakingUserControlViewModel(_toolStocktakingDataService, _userLogDataService, _settingsDataService);

        public static MuseumMainWindowViewModel MuseumMainWindowViewModel { get => _museumMainWindowViewModel; }
        public static ExhibitsUserControlViewModel ExhibitsUserControlViewModel { get => _exhibitsUserControlViewModel; }
        public static CatalogsUserControlViewModel CatalogsUserControlViewModel { get => _catalogsUserControlViewModel; }
        public static ExhibitsLocalizationsUserControlViewModel ExhibitsLocalizationsUserControlViewModel { get => _exhibitsLocalizationsUserControlViewModel; }
        public static ExhibitsStocktakingUserControlViewModel ExhibitsStocktakingUserControlViewModel { get => _exhibitsStocktakingUserControlViewModel; }
        public static ToolsUserControlViewModel ToolsUserControlViewModel { get => _toolsUserControlViewModel; }
        public static ToolsLocalizationsUserControlViewModel ToolsLocalizationsUserControlViewModel { get => _toolsLocalizationsUserControlViewModel; }
        public static UsersUserControlViewModel UsersUserControlViewModel { get => _usersUserControlViewModel; }
        public static UsersLogUserControlViewModel UsersLogUserControlViewModel { get => _usersLogUserControlViewModel; }
        public static ToolsStocktakingUserControlViewModel ToolsStocktakingUserControlViewModel { get => _toolsStocktakingUserControlViewModel; }

        //ADD OR UPDATE

        private static AddOrUpdateCatalogViewModel _addOrUpdateCatalogViewModel = new AddOrUpdateCatalogViewModel(_userDataService, _catalogDataService, _userLogDataService);
        private static AddOrUpdateExhibitLocalizationViewModel _addOrUpdateExhibitLocalizationViewModel = new AddOrUpdateExhibitLocalizationViewModel(_exhibitLocalizationDataService, _userLogDataService);
        private static AddOrUpdateExhibitViewModel _addOrUpdateExhibitViewModel = new AddOrUpdateExhibitViewModel(_exhibitDataService, _catalogDataService, _exhibitLocalizationDataService, _userLogDataService);
        private static AddOrUpdateToolViewModel _addOrUpdateToolViewModel = new AddOrUpdateToolViewModel(_toolDataService, _toolLocalizationDataService, _userLogDataService);
        private static AddOrUpdateToolsLocalizationViewModel _addOrUpdateToolsLocalizationViewModel = new AddOrUpdateToolsLocalizationViewModel(_toolLocalizationDataService, _userLogDataService);
        private static AddOrUpdateUserViewModel _addOrUpdateUserViewModel = new AddOrUpdateUserViewModel(_userDataService, _userLoginDataService, _userLogDataService);
        private static AddOrUpdateExhibitStocktakingViewModel _addOrUpdateExhibitStocktakingViewModel = new AddOrUpdateExhibitStocktakingViewModel(_exhibitStocktakingDataService, _exhibitDataService, _catalogDataService, _exhibitLocalizationDataService, _userLogDataService);
        private static AddOrUpdateToolsStocktakingViewModel _addOrUpdateToolsStocktakingViewModel = new AddOrUpdateToolsStocktakingViewModel(_toolDataService, _toolLocalizationDataService, _toolStocktakingDataService, _userLogDataService);

        public static AddOrUpdateCatalogViewModel AddOrUpdateCatalogViewModel { get => _addOrUpdateCatalogViewModel; }
        public static AddOrUpdateExhibitLocalizationViewModel AddOrUpdateExhibitLocalizationViewModel { get => _addOrUpdateExhibitLocalizationViewModel; }
        public static AddOrUpdateExhibitViewModel AddOrUpdateExhibitViewModel { get => _addOrUpdateExhibitViewModel; }
        public static AddOrUpdateToolViewModel AddOrUpdateToolViewModel { get => _addOrUpdateToolViewModel; }
        public static AddOrUpdateToolsLocalizationViewModel AddOrUpdateToolsLocalizationViewModel { get => _addOrUpdateToolsLocalizationViewModel; }
        public static AddOrUpdateUserViewModel AddOrUpdateUserViewModel { get => _addOrUpdateUserViewModel; }
        public static AddOrUpdateExhibitStocktakingViewModel AddOrUpdateExhibitStocktakingViewModel { get => _addOrUpdateExhibitStocktakingViewModel; }
        public static AddOrUpdateToolsStocktakingViewModel addOrUpdateToolsStocktakingViewModel { get => _addOrUpdateToolsStocktakingViewModel; }

        //LOGIN

        private static UserLoginViewModel _userLoginViewModel = new UserLoginViewModel(_userLoginDataService);

        public static UserLoginViewModel UserLoginViewModel { get => _userLoginViewModel; }
    }
}
