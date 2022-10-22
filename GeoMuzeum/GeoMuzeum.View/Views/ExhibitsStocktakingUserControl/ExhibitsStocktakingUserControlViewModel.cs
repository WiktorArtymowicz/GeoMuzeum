using GeoMuzeum.DataService;
using GeoMuzeum.Model;
using GeoMuzeum.View.ProjectHelpers;
using GeoMuzeum.View.Views.ExhibitsStocktakingUserControl.AddOrUpdateExhibitStocktaking;
using GeoMuzeum.View.Views.ToolsStocktakingUserControl.AddOrUpdateToolStocktaking;
using GeoMuzeum.View.ViewServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GeoMuzeum.View.Views.ExhibitsStocktakingUserControl
{
    public class ExhibitsStocktakingUserControlViewModel : BaseViewModel
    {
        private readonly IExhibitStocktakingDataService _exhibitStocktakingDataService;
        private readonly ISettingsDataService _settingsDataService;
        private readonly IUserLogDataService _userLogDataService;

        private User _sentUser;
        private Settings _settings;
        private int _collectionQuantity;

        public ExhibitsStocktakingUserControlViewModel()
        {

        }

        public ExhibitsStocktakingUserControlViewModel(IExhibitStocktakingDataService exhibitStocktakingDataService, IUserLogDataService userLogDataService, ISettingsDataService settingsDataService)
        {
            _exhibitStocktakingDataService = exhibitStocktakingDataService;
            _userLogDataService = userLogDataService;
            _settingsDataService = settingsDataService;

            ExhibitStocktackings = new ObservableCollection<ExhibitStocktaking>();
            ExhibitStocktakingSortTypes = new List<ExhibitStocktakingSortType>();
            ExhibitStocktakingSearchTypes = new List<ExhibitStocktakingSearchType>();
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
                await LoadExhibitStocktackings();
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
            ExhibitStocktakingSearchTypes.Clear();

            ExhibitStocktakingSearchTypes = Enum.GetValues(typeof(ExhibitStocktakingSearchType)).Cast<ExhibitStocktakingSearchType>().ToList();
            SelectedExhibitStocktakingSearchType = ExhibitStocktakingSearchTypes.FirstOrDefault();
        }

        private void LoadSortTypes()
        {
            ExhibitStocktakingSortTypes.Clear();

            ExhibitStocktakingSortTypes = Enum.GetValues(typeof(ExhibitStocktakingSortType)).Cast<ExhibitStocktakingSortType>().ToList();
            SelectedExhibitStocktakingSortType = ExhibitStocktakingSortTypes.FirstOrDefault();
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

        private async Task LoadExhibitStocktackings()
        {
            ExhibitStocktackings.Clear();

            var exhibitStocktakings = await _exhibitStocktakingDataService.GetAllExhibitStocktakingPositions();

            if (_collectionQuantity == exhibitStocktakings.Count)
                exhibitStocktakings = await _exhibitStocktakingDataService.GetAllExhibitStocktakingPositions();

            ExhibitStocktackings = exhibitStocktakings.ToObservableCollection();
            SelectedExhibitStocktaking = ExhibitStocktackings.FirstOrDefault();
        }

        private async void StartStocktaking()
        {
            try
            {
                var question = MessageBox.Show("Czy rozpocząć remanent eksponatów?", "Pytanie", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (question == MessageBoxResult.No)
                    return;

                _settings.IsExhibitStocktaking = true;
                await _settingsDataService.UpdateSettings(_settings);
                await _userLogDataService.AddUserLog(new UserLog($"Użytkownik rozpoczął remanent eksponatów.", _sentUser));

                await LoadDataAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private async void CancelStocktaking()
        {
            try
            {
                var question = MessageBox.Show("Czy anulować remanent eksponatów? Anulowanie remanentu spowoduje usunięcie wprowadzonych danych.", "Pytanie", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (question == MessageBoxResult.No)
                    return;


                await _exhibitStocktakingDataService.DeleteAllTable();
                await _userLogDataService.AddUserLog(new UserLog($"Użytkownik anulował remanent eksponatów.", _sentUser));

                _settings.IsExhibitStocktaking = false;
                await _settingsDataService.UpdateSettings(_settings);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await LoadDataAsync();
        }

        private async void ConfirmStocktaking()
        {
            try
            {
                var question = MessageBox.Show("Czy zatwierdzić remanent eksponatów?", "Pytanie", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (question == MessageBoxResult.No)
                    return;

                _settings.IsExhibitStocktaking = false;
                await _settingsDataService.UpdateSettings(_settings);

                await _exhibitStocktakingDataService.ConfirmStocktaking();
                await _userLogDataService.AddUserLog(new UserLog($"Użytkownik zatwierdził remanent eksponatów.", _sentUser));

                await LoadDataAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private async void AddStocktakingPosition()
        {
            SaveCollectionQuantity();

            ViewMessenger.Default.Send<ExhibitStocktaking>(null);

            var catalogViewService = new ViewDialogService<AddOrUpdateExhibitStocktakingView>(new AddOrUpdateExhibitStocktakingView(_sentUser));
            catalogViewService.ShowGenericWindow();

            await LoadDataAsync();
        }

        private async void UpdateStocktakingPosition()
        {
            if (SelectedExhibitStocktaking == null)
                return;

            SaveCollectionQuantity();

            ViewMessenger.Default.Send(SelectedExhibitStocktaking);

            var catalogViewService = new ViewDialogService<AddOrUpdateExhibitStocktakingView>(new AddOrUpdateExhibitStocktakingView(_sentUser));
            catalogViewService.ShowGenericWindow();

            await LoadDataAsync();
        }

        private async void DeleteStocktakingPosition()
        {
            try
            {
                if (SelectedExhibitStocktaking == null)
                    return;

                var question = MessageBox.Show("Czy usunąć zaznaczoną pozycję?", "Pytanie", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (question == MessageBoxResult.No)
                    return;

                await _exhibitStocktakingDataService.DeleteExhibitStocktakingPosition(SelectedExhibitStocktaking);
                await _userLogDataService.AddUserLog(new UserLog($"Użytkownik usunął pozycję remanentu eksponatów {SelectedExhibitStocktaking.Exhibit.ExhibitName}.", _sentUser));

                await LoadDataAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }            
        }

        private void SaveCollectionQuantity()
        {
            _collectionQuantity = ExhibitStocktackings.Count;
        }

        private async void SearchExhibitStocktakingsBy(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                await LoadSettings();
                await LoadExhibitStocktackings();
                return;
            }

            if (SelectedExhibitStocktakingSearchType == ExhibitStocktakingSearchType.Eksponat)
            {
                var searchedExhibits = await _exhibitStocktakingDataService.GetAllExhibitStocktakingPositionsByExhibit(searchText);
                ExhibitStocktackings = searchedExhibits.ToObservableCollection();
            }

            if (SelectedExhibitStocktakingSearchType == ExhibitStocktakingSearchType.Katalog)
            {
                var searchedExhibits = await _exhibitStocktakingDataService.GetAllExhibitStocktakingPositionsByCatalog(searchText);
                ExhibitStocktackings = searchedExhibits.ToObservableCollection();
            }

            if (SelectedExhibitStocktakingSearchType == ExhibitStocktakingSearchType.Lokalizacja)
            {
                var searchedExhibits = await _exhibitStocktakingDataService.GetAllExhibitStocktakingPositionsByLocalization(searchText);
                ExhibitStocktackings = searchedExhibits.ToObservableCollection();
            }
        }

        private void SortExhibitStocktakingsBy(ExhibitStocktakingSortType exhibitStocktakingSortType)
        {
            if (exhibitStocktakingSortType == ExhibitStocktakingSortType.Domyślnie)
                ExhibitStocktackings = ExhibitStocktackings.OrderBy(x => x.ExhibitStocktakingId).ToObservableCollection();

            if (exhibitStocktakingSortType == ExhibitStocktakingSortType.Katalog)
                ExhibitStocktackings = ExhibitStocktackings.OrderBy(x => x.Catalog.CatalogName).ToObservableCollection();

            if (exhibitStocktakingSortType == ExhibitStocktakingSortType.Lokalizacja)
                ExhibitStocktackings = ExhibitStocktackings.OrderBy(x => x.Localization.ExhibitLocalizationNumber).ToObservableCollection();

            if (exhibitStocktakingSortType == ExhibitStocktakingSortType.Eksponat)
                ExhibitStocktackings = ExhibitStocktackings.OrderBy(x => x.Exhibit.ExhibitName).ToObservableCollection();
        }


        private void PrintExhibitStocktakings()
        {
            try
            {
                var question = MessageBox.Show("Czy wydrukować widoczną listę remanentu eksponatów?", "Błąd", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (question == MessageBoxResult.No)
                    return;

                PrintService.PrintExhibitStocktakings(ExhibitStocktackings.ToList());

                MessageBox.Show("Przekazano listę do wydruku.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private ICommand _startStocktakingCommand;
        public ICommand StartStocktakingCommand
        {
            get
            {
                if (_startStocktakingCommand == null)
                    _startStocktakingCommand = new CustomCommand((object o) => { StartStocktaking(); }, (object o) => { return !IsExhibitStocktakingActive; });

                return _startStocktakingCommand;
            }
            set { _startStocktakingCommand = value; }
        }

        private ICommand _cancelStocktakingCommand;
        public ICommand CancelStocktakingCommand
        {
            get
            {
                if (_cancelStocktakingCommand == null)
                    _cancelStocktakingCommand = new CustomCommand((object o) => { CancelStocktaking(); }, (object o) => { return IsExhibitStocktakingActive; });

                return _cancelStocktakingCommand;
            }
            set { _cancelStocktakingCommand = value; }
        }

        private ICommand _confirmStocktakingCommand;
        public ICommand ConfirmStocktakingCommand
        {
            get
            {
                if (_confirmStocktakingCommand == null)
                    _confirmStocktakingCommand = new CustomCommand((object o) => { ConfirmStocktaking(); }, (object o) => { return IsExhibitStocktakingActive && ExhibitStocktackings.Any(); });

                return _confirmStocktakingCommand;
            }
            set { _confirmStocktakingCommand = value; }
        }

        private ICommand _addStocktakingPositionCommand;
        public ICommand AddStocktakingPositionCommand
        {
            get
            {
                if (_addStocktakingPositionCommand == null)
                    _addStocktakingPositionCommand = new CustomCommand((object o) => { AddStocktakingPosition(); }, (object o) => { return IsExhibitStocktakingActive; });

                return _addStocktakingPositionCommand;
            }
            set { _addStocktakingPositionCommand = value; }
        }

        private ICommand _updateStocktakingPositionCommand;
        public ICommand UpdateStocktakingPositionCommand
        {
            get
            {
                if (_updateStocktakingPositionCommand == null)
                    _updateStocktakingPositionCommand = new CustomCommand((object o) => { UpdateStocktakingPosition(); }, (object o) => { return IsExhibitStocktakingActive && SelectedExhibitStocktaking != null; });

                return _updateStocktakingPositionCommand;
            }
            set { _updateStocktakingPositionCommand = value; }
        }

        private ICommand _deleteStocktakingPositionCommand;
        public ICommand DeleteStocktakingPositionCommand
        {
            get
            {
                if (_deleteStocktakingPositionCommand == null)
                    _deleteStocktakingPositionCommand = new CustomCommand((object o) => { DeleteStocktakingPosition(); }, (object o) => { return IsExhibitStocktakingActive && SelectedExhibitStocktaking != null; });

                return _deleteStocktakingPositionCommand;
            }
            set { _deleteStocktakingPositionCommand = value; }
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

        private ICommand _printCommand;
        public ICommand PrintCommand
        {
            get
            {
                if (_printCommand == null)
                    _printCommand = new CustomCommand((object o) => { PrintExhibitStocktakings(); }, (object o) => { return IsExhibitStocktakingActive && ExhibitStocktackings.Any(); });

                return _printCommand;
            }
            set { _printCommand = value; }
        }

        private ObservableCollection<ExhibitStocktaking> _exhibitStocktakings;
        public ObservableCollection<ExhibitStocktaking> ExhibitStocktackings
        {
            get { return _exhibitStocktakings; }
            set { _exhibitStocktakings = value; OnPropertyChanged(nameof(ExhibitStocktackings)); }
        }

        private ExhibitStocktaking _selectedExhibitStocktaking;
        public ExhibitStocktaking SelectedExhibitStocktaking
        {
            get { return _selectedExhibitStocktaking; }
            set { _selectedExhibitStocktaking = value; OnPropertyChanged(nameof(SelectedExhibitStocktaking)); }
        }

        private List<ExhibitStocktakingSortType> _exhibitStocktakingSortTypes;
        public List<ExhibitStocktakingSortType> ExhibitStocktakingSortTypes
        {
            get { return _exhibitStocktakingSortTypes; }
            set { _exhibitStocktakingSortTypes = value; OnPropertyChanged(nameof(ExhibitStocktakingSortTypes)); }
        }

        private ExhibitStocktakingSortType _selectedExhibitStocktakingSortType;
        public ExhibitStocktakingSortType SelectedExhibitStocktakingSortType
        {
            get { return _selectedExhibitStocktakingSortType; }
            set
            {
                _selectedExhibitStocktakingSortType = value;
                OnPropertyChanged(nameof(SelectedExhibitStocktakingSortType));
                SortExhibitStocktakingsBy(SelectedExhibitStocktakingSortType);
            }
        }

        private List<ExhibitStocktakingSearchType> _exhibitStocktakingSearchTypes;
        public List<ExhibitStocktakingSearchType> ExhibitStocktakingSearchTypes
        {
            get { return _exhibitStocktakingSearchTypes; }
            set { _exhibitStocktakingSearchTypes = value; OnPropertyChanged(nameof(ExhibitStocktakingSearchTypes)); }
        }

        private ExhibitStocktakingSearchType _selectedExhibitStocktakingSearchType;
        public ExhibitStocktakingSearchType SelectedExhibitStocktakingSearchType
        {
            get { return _selectedExhibitStocktakingSearchType; }
            set { _selectedExhibitStocktakingSearchType = value; OnPropertyChanged(nameof(SelectedExhibitStocktakingSearchType)); }
        }

        private bool _isExhibitStocktakingActive;
        public bool IsExhibitStocktakingActive
        {
            get { return _isExhibitStocktakingActive; }
            set { _isExhibitStocktakingActive = value; OnPropertyChanged(nameof(IsExhibitStocktakingActive)); }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                SearchExhibitStocktakingsBy(SearchText);
            }
        }
    }
}
