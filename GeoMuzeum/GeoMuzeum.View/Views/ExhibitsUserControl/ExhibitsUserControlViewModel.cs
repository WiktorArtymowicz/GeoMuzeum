using GeoMuzeum.DataService;
using GeoMuzeum.Model;
using GeoMuzeum.View.ProjectHelpers;
using GeoMuzeum.View.Views.ExhibitsUserControl.AddOrUpdateExhibit;
using GeoMuzeum.View.ViewServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;


namespace GeoMuzeum.View.Views.ExhibitsUserControl
{
    public class ExhibitsUserControlViewModel : BaseViewModel
    {
        private IExhibitDataService _exhibitDataService;
        private readonly IUserLogDataService _userLogDataService;
        private readonly ISettingsDataService _settingsDataService;

        private User _sentUser;
        private Settings _settings;

        protected ExhibitsUserControlViewModel()
        {

        }

        public ExhibitsUserControlViewModel(IExhibitDataService exhibitDataService, IUserLogDataService userLogDataService, ISettingsDataService settingsDataService)
        {
            _exhibitDataService = exhibitDataService;
            _userLogDataService = userLogDataService;
            _settingsDataService = settingsDataService;

            Exhibits = new ObservableCollection<Exhibit>();

            ExhibitSearchTypes = new List<ExhibitSearchType>();
            ExhibitSortTypes = new List<ExhibitSortType>();
        }

        private async Task LoadSettings()
        {
            var settings = await _settingsDataService.GetSettings();

            if (settings == null)
            {
                _settings = new Settings();
                await _settingsDataService.CreateSettings(_settings);
            }
            else
            {
                _settings = settings;
                IsExhibitStocktakingActive = _settings.IsExhibitStocktaking;
            }
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
            try
            {
                await LoadSettings();
                await LoadExhibits();
                LoadSearchTypes();
                LoadSortTypes();

                SearchText = string.Empty;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void LoadSortTypes()
        {
            ExhibitSortTypes.Clear();

            ExhibitSortTypes = Enum.GetValues(typeof(ExhibitSortType)).Cast<ExhibitSortType>().ToList();
            SelectedExhibitSortType = ExhibitSortTypes.FirstOrDefault();
        }

        private void LoadSearchTypes()
        {
            ExhibitSearchTypes.Clear();

            ExhibitSearchTypes = Enum.GetValues(typeof(ExhibitSearchType)).Cast<ExhibitSearchType>().ToList();
            SelectedExhibitSearchType = ExhibitSearchTypes.FirstOrDefault();
        }

        private async Task LoadExhibits()
        {
            Exhibits.Clear();

            var exhibitsFromDb = await _exhibitDataService.GetAllExhibitsAsync();
            Exhibits = exhibitsFromDb.ToObservableCollection();
        }

        private async void SearchExhibitsBy(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                await LoadExhibits();
                return;
            }

            if (SelectedExhibitSearchType == ExhibitSearchType.Nazwa)
            {
                var searchedExhibits = await _exhibitDataService.GetAllExhibitsByNameAsync(searchText);
                Exhibits = searchedExhibits.ToObservableCollection();
            }

            if (SelectedExhibitSearchType == ExhibitSearchType.Typ)
            {
                var searchedExhibits = await _exhibitDataService.GetAllExhibitsByTypeAsync(searchText);
                Exhibits = searchedExhibits.ToObservableCollection();
            }

            if (SelectedExhibitSearchType == ExhibitSearchType.Lokalizacja)
            {
                var searchedExhibits = await _exhibitDataService.GetAllExhibitsByLocalizationAsync(searchText);
                Exhibits = searchedExhibits.ToObservableCollection();
            }
        }

        private void SortExhibitsBy(ExhibitSortType exhibitSortType)
        {
            if (exhibitSortType == ExhibitSortType.Domyślnie)
                Exhibits = Exhibits.OrderBy(x => x.ExhibitId).ToObservableCollection();

            if (exhibitSortType == ExhibitSortType.Katalog)
                Exhibits = Exhibits.OrderBy(x => x.Catalog.CatalogName).ToObservableCollection();

            if (exhibitSortType == ExhibitSortType.Lokalizacja)
                Exhibits = Exhibits.OrderBy(x => x.Localization.ExhibitLocalizationNumber).ToObservableCollection();

            if (exhibitSortType == ExhibitSortType.Nazwa)
                Exhibits = Exhibits.OrderBy(x => x.ExhibitName).ToObservableCollection();

            if (exhibitSortType == ExhibitSortType.Opis)
                Exhibits = Exhibits.OrderBy(x => x.ExhibitDescription).ToObservableCollection();

            if (exhibitSortType == ExhibitSortType.Typ)
                Exhibits = Exhibits.OrderBy(x => x.ExhibitTypeInfo).ToObservableCollection();
        }

        private async void AddExhibit()
        {
            ViewMessenger.Default.Send<Exhibit>(null);

            var catalogViewService = new ViewDialogService<AddOrUpdateExhibitView>(new AddOrUpdateExhibitView(_sentUser));
            catalogViewService.ShowGenericWindow();

            await LoadDataAsync();
        }

        private async void UpdateExhibit()
        {
            ViewMessenger.Default.Send(SelectedExhibit);

            var catalogViewService = new ViewDialogService<AddOrUpdateExhibitView>(new AddOrUpdateExhibitView(_sentUser));
            catalogViewService.ShowGenericWindow();

            await LoadDataAsync();
        }

        private async void DeleteExhibit()
        {
            try
            {
                if (SelectedExhibit == null)
                    return;

                var question = MessageBox.Show("Czy usunać eksponat?", "Błąd", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (question == MessageBoxResult.No)
                    return;

                await _exhibitDataService.DeleteExhibit(SelectedExhibit);
                await _userLogDataService.AddUserLog(new UserLog($"Użytkownik usunął eksponat {SelectedExhibit.ExhibitName}.", _sentUser));

                await LoadDataAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void PrintExhibits()
        {
            try
            {
                var question = MessageBox.Show("Czy wydrukować widoczną listę eksponatów?", "Błąd", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (question == MessageBoxResult.No)
                    return;

                PrintService.PrintExhibits(Exhibits.ToList());

                MessageBox.Show("Przekazano eksponaty do wydruku.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private ICommand _addExhibitCommand;
        public ICommand AddExhibitCommand
        {
            get
            {
                if (_addExhibitCommand == null)
                    _addExhibitCommand = new CustomCommand((object o) => { AddExhibit(); }, (object o) => { return true; });

                return _addExhibitCommand;
            }
            set { _addExhibitCommand = value; }
        }

        private ICommand _updateExhibitCommand;
        public ICommand UpdateExhibitCommand
        {
            get
            {
                if (_updateExhibitCommand == null)
                    _updateExhibitCommand = new CustomCommand((object o) => { UpdateExhibit(); }, (object o) => { return !IsExhibitStocktakingActive; });

                return _updateExhibitCommand;
            }
            set { _updateExhibitCommand = value; }
        }

        private ICommand _deleteExhibitCommand;
        public ICommand DeleteExhibitCommand
        {
            get
            {
                if (_deleteExhibitCommand == null)
                    _deleteExhibitCommand = new CustomCommand((object o) => { DeleteExhibit(); }, (object o) => { return !IsExhibitStocktakingActive; });

                return _deleteExhibitCommand;
            }
            set { _deleteExhibitCommand = value; }
        }

        private ICommand _printCommand;
        public ICommand PrintCommand
        {
            get
            {
                if (_printCommand == null)
                    _printCommand = new CustomCommand((object o) => { PrintExhibits(); }, (object o) => { return true; });

                return _printCommand;
            }
            set { _printCommand = value; }
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

        private ObservableCollection<Exhibit> _exhibits;
        public ObservableCollection<Exhibit> Exhibits
        {
            get { return _exhibits; }
            set { _exhibits = value; OnPropertyChanged(nameof(Exhibits)); }
        }

        private Exhibit _selectedExhibit;
        public Exhibit SelectedExhibit
        {
            get { return _selectedExhibit; }
            set { _selectedExhibit = value; OnPropertyChanged(nameof(SelectedExhibit)); }
        }

        private List<ExhibitSearchType> _exhibitSearchTypes;
        public List<ExhibitSearchType> ExhibitSearchTypes
        {
            get { return _exhibitSearchTypes; }
            set { _exhibitSearchTypes = value; OnPropertyChanged(nameof(ExhibitSearchTypes)); }
        }

        private ExhibitSearchType _selectedExhibitSearchType;
        public ExhibitSearchType SelectedExhibitSearchType
        {
            get { return _selectedExhibitSearchType; }
            set { _selectedExhibitSearchType = value; OnPropertyChanged(nameof(SelectedExhibitSearchType)); }
        }

        private List<ExhibitSortType> _exhibitSortTypes;
        public List<ExhibitSortType> ExhibitSortTypes
        {
            get { return _exhibitSortTypes; }
            set { _exhibitSortTypes = value; OnPropertyChanged(nameof(ExhibitSortTypes)); }
        }

        private ExhibitSortType _selectedExhibitSortType;
        public ExhibitSortType SelectedExhibitSortType
        {
            get { return _selectedExhibitSortType; }
            set
            {
                _selectedExhibitSortType = value;
                OnPropertyChanged(nameof(SelectedExhibitSortType));
                SortExhibitsBy(SelectedExhibitSortType);
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
                SearchExhibitsBy(SearchText);
            }
        }

        private bool _isExhibitStocktakingActive;
        public bool IsExhibitStocktakingActive
        {
            get { return _isExhibitStocktakingActive; }
            set { _isExhibitStocktakingActive = value; OnPropertyChanged(nameof(IsExhibitStocktakingActive)); }
        }
    }
}
