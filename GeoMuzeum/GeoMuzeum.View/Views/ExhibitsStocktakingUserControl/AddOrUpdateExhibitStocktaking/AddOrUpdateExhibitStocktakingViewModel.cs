using GeoMuzeum.DataService;
using GeoMuzeum.Model;
using GeoMuzeum.View.Enums;
using GeoMuzeum.View.ProjectHelpers;
using GeoMuzeum.View.ViewServices;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GeoMuzeum.View.Views.ExhibitsStocktakingUserControl.AddOrUpdateExhibitStocktaking
{
    public class AddOrUpdateExhibitStocktakingViewModel : BaseViewModel
    {
        private readonly IExhibitStocktakingDataService _exhibitStocktakingDataService;
        private readonly IExhibitDataService _exhibitDataService;
        private readonly ICatalogDataService _catalogDataService;
        private readonly IExhibitLocalizationDataService _exhibitLocalizationDataService;
        private readonly IUserLogDataService _userLogDataService;

        private ExhibitStocktaking _sentExhibitStocktaking;
        private User _sentUser;

        public Action CloseAction { get; set; }

        protected AddOrUpdateExhibitStocktakingViewModel()
        {

        }

        public AddOrUpdateExhibitStocktakingViewModel(IExhibitStocktakingDataService exhibitStocktakingDataService, IExhibitDataService exhibitDataService, ICatalogDataService catalogDataService, IExhibitLocalizationDataService exhibitLocalizationDataService, IUserLogDataService userLogDataService)
        {
            _exhibitStocktakingDataService = exhibitStocktakingDataService;
            _exhibitDataService = exhibitDataService;
            _catalogDataService = catalogDataService;
            _exhibitLocalizationDataService = exhibitLocalizationDataService;
            _userLogDataService = userLogDataService;

            Exhibits = new ObservableCollection<Exhibit>();
            ExhibitLocalizations = new ObservableCollection<ExhibitLocalization>();
            Catalogs = new ObservableCollection<Catalog>();

            ViewMessenger.Default.Register<ExhibitStocktaking>(this, OnExhibitStocktakingRecived);
        }

        public override void SetUser(User user)
        {
            if (user == null)
            {
                MessageBox.Show("Błąd przy przekazywaniu użytkownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                CloseAction();
            }

            _sentUser = user;
        }

        public async override Task LoadDataAsync()
        {
            try
            {
                var exhibitStocktakings = await _exhibitStocktakingDataService.GetAllExhibitStocktakingPositions();
                var exhibitIds = exhibitStocktakings.Select(x => x.Exhibit.ExhibitId).ToList();

                ExhibitStocktaking = _sentExhibitStocktaking == null ? new ExhibitStocktaking() : await _exhibitStocktakingDataService.FindExhibitStocktakingPositionById(_sentExhibitStocktaking);

                var exhibits = await _exhibitDataService.GetAllExhibitsAsync();

                Exhibits = EditStatusType == EditStatusType.Add
                    ? exhibits.Where(x => !exhibitIds.Any(y => y == x.ExhibitId)).ToObservableCollection()
                    : exhibits.ToObservableCollection();

                SelectedExhibit = ExhibitStocktaking.Exhibit == null ? Exhibits.FirstOrDefault() : Exhibits.FirstOrDefault(x => x.ExhibitId == ExhibitStocktaking.Exhibit.ExhibitId);

                var exhibitLocalizations = await _exhibitLocalizationDataService.GetExhibitLocalizations();
                ExhibitLocalizations = exhibitLocalizations.ToObservableCollection();
                SelectedExhibitLocalization = ExhibitStocktaking.Localization == null ? ExhibitLocalizations.FirstOrDefault() : ExhibitLocalizations.FirstOrDefault(x => x.ExhibitLocalizationId == ExhibitStocktaking.Localization.ExhibitLocalizationId);

                var catalogs = await _catalogDataService.GetAllCatalogs();
                Catalogs = catalogs.ToObservableCollection();
                SelectedCatalog = ExhibitStocktaking.Catalog == null ? Catalogs.FirstOrDefault() : Catalogs.FirstOrDefault(x => x.CatalogId == ExhibitStocktaking.Catalog.CatalogId);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void OnExhibitStocktakingRecived(ExhibitStocktaking exhibitStocktaking)
        {
            _sentExhibitStocktaking = exhibitStocktaking;
            _editStatusType = _sentExhibitStocktaking == null ? EditStatusType.Add : EditStatusType.Modify;
        }

        private async void AddOrUpdateExhibitStocktaking(ExhibitStocktaking exhibitStocktaking)
        {
            try
            {
                if (SelectedExhibit == null || SelectedExhibitLocalization == null)
                {
                    MessageBox.Show("Nie zostało wybrane narzędzie lub lokalizacja.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (_editStatusType == EditStatusType.Add)
                {
                    await _exhibitStocktakingDataService.AddExhibitStocktakingPosition(exhibitStocktaking);
                    await _userLogDataService.AddUserLog(new UserLog($"Użytkownik dodał pozycję do remanentu eksponatów {exhibitStocktaking.Exhibit.ExhibitName}.", _sentUser));
                }

                else
                {
                    await _exhibitStocktakingDataService.UpdateExhibitStocktakingPosition(exhibitStocktaking);
                    await _userLogDataService.AddUserLog(new UserLog($"Użytkownik edytował pozycję remanentu eksponatów {exhibitStocktaking.Exhibit.ExhibitName}.", _sentUser));
                }

                CloseWindow();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private ExhibitStocktaking FillExhibitStocktakingToSave()
        {
            ExhibitStocktaking.Catalog = SelectedCatalog;
            ExhibitStocktaking.Exhibit = SelectedExhibit;
            ExhibitStocktaking.Localization = SelectedExhibitLocalization;

            return ExhibitStocktaking;
        }

        private void CloseWindow()
        {
            CloseAction();
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
                    _saveCommand = new CustomCommand((object o) => { AddOrUpdateExhibitStocktaking(FillExhibitStocktakingToSave()); }, (object o) => { return true; });

                return _saveCommand;
            }
            set { _saveCommand = value; }
        }

        private ExhibitStocktaking _exhibitStocktaking;
        public ExhibitStocktaking ExhibitStocktaking
        {
            get { return _exhibitStocktaking; }
            set { _exhibitStocktaking = value; }
        }

        private ObservableCollection<Exhibit> _exhibits;
        public ObservableCollection<Exhibit> Exhibits
        {
            get { return _exhibits; }
            set { _exhibits = value; OnPropertyChanged(nameof(Exhibits)); }
        }

        private Exhibit _selectedExhinbit;
        public Exhibit SelectedExhibit
        {
            get { return _selectedExhinbit; }
            set { _selectedExhinbit = value; OnPropertyChanged(nameof(SelectedExhibit)); }
        }

        private ObservableCollection<ExhibitLocalization> _exhibitLocalizations;
        public ObservableCollection<ExhibitLocalization> ExhibitLocalizations
        {
            get { return _exhibitLocalizations; }
            set { _exhibitLocalizations = value; OnPropertyChanged(nameof(ExhibitLocalizations)); }
        }

        private ExhibitLocalization _selectedExhibitLocalization;
        public ExhibitLocalization SelectedExhibitLocalization
        {
            get { return _selectedExhibitLocalization; }
            set { _selectedExhibitLocalization = value; OnPropertyChanged(nameof(SelectedExhibitLocalization)); }
        }

        private ObservableCollection<Catalog> _catalogs;
        public ObservableCollection<Catalog> Catalogs
        {
            get { return _catalogs; }
            set { _catalogs = value; OnPropertyChanged(nameof(Catalogs)); }
        }

        private Catalog _selectedCatalog;
        public Catalog SelectedCatalog
        {
            get { return _selectedCatalog; }
            set { _selectedCatalog = value; OnPropertyChanged(nameof(SelectedCatalog)); }
        }

        private EditStatusType _editStatusType;
        public EditStatusType EditStatusType
        {
            get { return _editStatusType; }
            set { _editStatusType = value; }
        }
    }
}
