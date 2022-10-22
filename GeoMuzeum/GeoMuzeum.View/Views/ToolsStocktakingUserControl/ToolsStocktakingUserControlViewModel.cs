using GeoMuzeum.DataService;
using GeoMuzeum.Model;
using GeoMuzeum.View.ProjectHelpers;
using GeoMuzeum.View.Views.ToolsStocktakingUserControl.AddOrUpdateToolStocktaking;
using GeoMuzeum.View.ViewServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GeoMuzeum.View.Views.ToolsStocktakingUserControl
{
    public class ToolsStocktakingUserControlViewModel : BaseViewModel
    {
        private readonly IToolStocktakingDataService _toolStocktakingDataService;
        private readonly IUserLogDataService _userLogDataService;
        private readonly ISettingsDataService _settingsDataService;

        private User _sentUser;
        private Settings _settings;
        private int _collectionQuantity;

        protected ToolsStocktakingUserControlViewModel()
        {

        }

        public ToolsStocktakingUserControlViewModel(IToolStocktakingDataService toolStocktakingDataService, IUserLogDataService userLogDataService, ISettingsDataService settingsDataService)
        {
            _toolStocktakingDataService = toolStocktakingDataService;
            _userLogDataService = userLogDataService;
            _settingsDataService = settingsDataService;

            ToolStocktakings = new ObservableCollection<ToolStocktaking>();
            ToolStocktakingSortTypes = new List<ToolStocktakingSortType>();
            ToolStocktakingSearchTypes = new List<ToolStocktakingSearchType>();
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

        public async override Task LoadDataAsync()
        {
            try
            {
                await LoadSettings();
                await LoadToolStocktakings();
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
            ToolStocktakingSearchTypes.Clear();

            ToolStocktakingSearchTypes = Enum.GetValues(typeof(ToolStocktakingSearchType)).Cast<ToolStocktakingSearchType>().ToList();
            SelectedToolStocktakingSearchType = ToolStocktakingSearchTypes.FirstOrDefault();
        }

        private void LoadSortTypes()
        {
            ToolStocktakingSortTypes.Clear();

            ToolStocktakingSortTypes = Enum.GetValues(typeof(ToolStocktakingSortType)).Cast<ToolStocktakingSortType>().ToList();
            SelectedToolStocktakingSortType = ToolStocktakingSortTypes.FirstOrDefault();
        }

        private async Task LoadToolStocktakings()
        {
            ToolStocktakings.Clear();

            var toolStocktakings = await _toolStocktakingDataService.GetAllToolsStocktakings();

            if (_collectionQuantity == ToolStocktakings.Count)
                toolStocktakings = await _toolStocktakingDataService.GetAllToolsStocktakings();

            ToolStocktakings = toolStocktakings.ToObservableCollection();
            SelectedToolStocktaking = ToolStocktakings.FirstOrDefault();
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
                IsToolStocktakingActive = settings.IsToolStocktaking;
            }
        }

        private void SaveCollectionQuantity()
        {
            _collectionQuantity = ToolStocktakings.Count;
        }

        private async void SearchToolStocktakingsBy(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                await LoadSettings();
                await LoadToolStocktakings();
                return;
            }

            if (SelectedToolStocktakingSearchType == ToolStocktakingSearchType.Narzędzie)
            {
                var searchedTools = await _toolStocktakingDataService.GetAllToolsStocktakingsByTool(searchText);
                ToolStocktakings = searchedTools.ToObservableCollection();
            }

            if (SelectedToolStocktakingSearchType == ToolStocktakingSearchType.Lokalizacja)
            {
                var searchedTools = await _toolStocktakingDataService.GetAllToolsStocktakingsByLocalization(searchText);
                ToolStocktakings = searchedTools.ToObservableCollection();
            }
        }

        private void SortToolStocktakingsBy(ToolStocktakingSortType toolStocktakingSortType)
        {
            if (toolStocktakingSortType == ToolStocktakingSortType.Domyślnie)
                ToolStocktakings = ToolStocktakings.OrderBy(x => x.ToolStocktakingId).ToObservableCollection();

            if (toolStocktakingSortType == ToolStocktakingSortType.Lokalizacja)
                ToolStocktakings = ToolStocktakings.OrderBy(x => x.Localization.ToolLocalizationNumber).ToObservableCollection();

            if (toolStocktakingSortType == ToolStocktakingSortType.Narzędzie)
                ToolStocktakings = ToolStocktakings.OrderBy(x => x.Tool.ToolName).ToObservableCollection();
        }

        private async void StartStocktaking()
        {
            try
            {
                var question = MessageBox.Show("Czy rozpocząć remanent narzędzi?", "Pytanie", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (question == MessageBoxResult.No)
                    return;

                _settings.IsToolStocktaking = true;
                await _settingsDataService.UpdateSettings(_settings);

                await _userLogDataService.AddUserLog(new UserLog($"Użytkownik rozpoczął remanent narzędzi.", _sentUser));

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
            var question = MessageBox.Show("Czy anulować remanent narzędzi? Anulowanie remanentu spowoduje usunięcie wprowadzonych danych.", "Pytanie", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (question == MessageBoxResult.No)
                return;

            try
            {
                await _toolStocktakingDataService.DeleteAllTable();

                _settings.IsToolStocktaking = false;
                await _settingsDataService.UpdateSettings(_settings);

                await _userLogDataService.AddUserLog(new UserLog($"Użytkownik anulował remanent narzędzi.", _sentUser));
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
                var question = MessageBox.Show("Czy zatwierdzić remanent narzędzi?", "Pytanie", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (question == MessageBoxResult.No)
                    return;

                _settings.IsToolStocktaking = false;
                await _settingsDataService.UpdateSettings(_settings);

                await _toolStocktakingDataService.ConfirmStocktaking();

                await _userLogDataService.AddUserLog(new UserLog($"Użytkownik zatwierdził remanent narzędzi.", _sentUser));

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

            ViewMessenger.Default.Send<ToolStocktaking>(null);

            var catalogViewService = new ViewDialogService<AddOrUpdateToolsStocktakingView>(new AddOrUpdateToolsStocktakingView(_sentUser));
            catalogViewService.ShowGenericWindow();

            await LoadDataAsync();
        }

        private async void UpdateStocktakingPositionAsync()
        {
            if (SelectedToolStocktaking == null)
                return;

            SaveCollectionQuantity();

            ViewMessenger.Default.Send(SelectedToolStocktaking);

            var catalogViewService = new ViewDialogService<AddOrUpdateToolsStocktakingView>(new AddOrUpdateToolsStocktakingView(_sentUser));
            catalogViewService.ShowGenericWindow();

            await LoadDataAsync();
        }

        private async void DeleteStocktakingPosition()
        {
            try
            {
                if (SelectedToolStocktaking == null) ;
                return;

                var question = MessageBox.Show("Czy usunąć zaznaczoną pozycję?", "Pytanie", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (question == MessageBoxResult.No)
                    return;

                await _toolStocktakingDataService.DeleteToolStocktakingPosition(SelectedToolStocktaking);

                await _userLogDataService.AddUserLog(new UserLog($"Użytkownik edytował usunął remanentu narzędzi {SelectedToolStocktaking.Tool.ToolName}.", _sentUser)); ;

                await LoadDataAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void PrintToolStocktakings()
        {
            try
            {
                var question = MessageBox.Show("Czy wydrukować widoczną listę remanentu narzędzi?", "Błąd", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (question == MessageBoxResult.No)
                    return;

                PrintService.PrintToolStocktakings(ToolStocktakings.ToList());

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
                    _startStocktakingCommand = new CustomCommand((object o) => { StartStocktaking(); }, (object o) => { return !IsToolStocktakingActive; });

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
                    _cancelStocktakingCommand = new CustomCommand((object o) => { CancelStocktaking(); }, (object o) => { return IsToolStocktakingActive; });

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
                    _confirmStocktakingCommand = new CustomCommand((object o) => { ConfirmStocktaking(); }, (object o) => { return IsToolStocktakingActive && ToolStocktakings.Any(); });

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
                    _addStocktakingPositionCommand = new CustomCommand((object o) => { AddStocktakingPosition(); }, (object o) => { return IsToolStocktakingActive; });

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
                    _updateStocktakingPositionCommand = new CustomCommand((object o) => { UpdateStocktakingPositionAsync(); }, (object o) => { return IsToolStocktakingActive && SelectedToolStocktaking != null; });

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
                    _deleteStocktakingPositionCommand = new CustomCommand((object o) => { DeleteStocktakingPosition(); }, (object o) => { return IsToolStocktakingActive && SelectedToolStocktaking != null; });

                return _deleteStocktakingPositionCommand;
            }
            set { _deleteStocktakingPositionCommand = value; }
        }

        private ICommand _printCommand;
        public ICommand PrintCommand
        {
            get
            {
                if (_printCommand == null)
                    _printCommand = new CustomCommand((object o) => { PrintToolStocktakings(); }, (object o) => { return IsToolStocktakingActive && ToolStocktakings.Any(); });

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

        private ObservableCollection<ToolStocktaking> _toolStocktakings;
        public ObservableCollection<ToolStocktaking> ToolStocktakings
        {
            get { return _toolStocktakings; }
            set { _toolStocktakings = value; OnPropertyChanged(nameof(ToolStocktakings)); }
        }

        private ToolStocktaking _selectedToolStocktaking;
        public ToolStocktaking SelectedToolStocktaking
        {
            get { return _selectedToolStocktaking; }
            set { _selectedToolStocktaking = value; OnPropertyChanged(nameof(SelectedToolStocktaking)); }
        }

        private List<ToolStocktakingSortType> _toolStocktakingSortTypes;
        public List<ToolStocktakingSortType> ToolStocktakingSortTypes
        {
            get { return _toolStocktakingSortTypes; }
            set { _toolStocktakingSortTypes = value; OnPropertyChanged(nameof(ToolStocktakingSortTypes)); }
        }

        private ToolStocktakingSortType _selectedToolStocktakingSortType;
        public ToolStocktakingSortType SelectedToolStocktakingSortType
        {
            get { return _selectedToolStocktakingSortType; }
            set
            {
                _selectedToolStocktakingSortType = value;
                OnPropertyChanged(nameof(SelectedToolStocktakingSortType));
                SortToolStocktakingsBy(SelectedToolStocktakingSortType);
            }
        }

        private List<ToolStocktakingSearchType> _toolStocktakingSearchTypes;
        public List<ToolStocktakingSearchType> ToolStocktakingSearchTypes
        {
            get { return _toolStocktakingSearchTypes; }
            set { _toolStocktakingSearchTypes = value; OnPropertyChanged(nameof(ToolStocktakingSearchTypes)); }
        }

        private ToolStocktakingSearchType _selectedToolStocktakingSearchType;
        public ToolStocktakingSearchType SelectedToolStocktakingSearchType
        {
            get { return _selectedToolStocktakingSearchType; }
            set { _selectedToolStocktakingSearchType = value; OnPropertyChanged(nameof(SelectedToolStocktakingSearchType)); }
        }

        private bool _isToolStocktakingActive;
        public bool IsToolStocktakingActive
        {
            get { return _isToolStocktakingActive; }
            set { _isToolStocktakingActive = value; OnPropertyChanged(nameof(IsToolStocktakingActive)); }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                SearchToolStocktakingsBy(SearchText);
            }
        }
    }
}
