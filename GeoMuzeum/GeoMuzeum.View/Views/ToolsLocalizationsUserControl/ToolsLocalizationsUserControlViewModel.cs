using GeoMuzeum.DataService;
using GeoMuzeum.Model;
using GeoMuzeum.View.ProjectHelpers;
using GeoMuzeum.View.Views.ToolsLocalizationsUserControl.AddOrUpdateToolsLocalization;
using GeoMuzeum.View.ViewServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GeoMuzeum.View.Views.ToolsLocalizationsUserControl
{
    public class ToolsLocalizationsUserControlViewModel : BaseViewModel
    {
        private readonly IToolLocalizationDataService _toolLocalizationDataService;
        private readonly IUserLogDataService _userLogDataService;

        private User _sentUser;

        protected ToolsLocalizationsUserControlViewModel()
        {

        }

        public ToolsLocalizationsUserControlViewModel(IToolLocalizationDataService toolLocalizationDataService, IUserLogDataService userLogDataService)
        {
            _toolLocalizationDataService = toolLocalizationDataService;
            _userLogDataService = userLogDataService;

            ToolsLocalizations = new ObservableCollection<ToolLocalization>();
            ToolLocalizationSearchTypes = new List<ToolLocalizationSearchType>();
            ToolLocalizationSortTypes = new List<ToolLocalizationSortType>();
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
                await LoadToolsLocalizations();
                LoadSearchTypes();
                LoadSortTypes();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void LoadSortTypes()
        {
            ToolLocalizationSortTypes.Clear();

            ToolLocalizationSortTypes = Enum.GetValues(typeof(ToolLocalizationSortType)).Cast<ToolLocalizationSortType>().ToList();
            SelectedToolLocalizationSortType = ToolLocalizationSortTypes.FirstOrDefault();
        }

        private void LoadSearchTypes()
        {
            ToolLocalizationSearchTypes.Clear();

            ToolLocalizationSearchTypes = Enum.GetValues(typeof(ToolLocalizationSearchType)).Cast<ToolLocalizationSearchType>().ToList();
            SelectedToolLocalizationSearchType = ToolLocalizationSearchTypes.FirstOrDefault();
        }

        private async Task LoadToolsLocalizations()
        {
            ToolsLocalizations.Clear();

            var toolsLocalizationsFromDb = await _toolLocalizationDataService.GetAllToolLocalizations();
            ToolsLocalizations = toolsLocalizationsFromDb.ToObservableCollection();

            SelectedToolLocalization = ToolsLocalizations.FirstOrDefault();
        }

        private async void SearchLocalizationsBy(string searchText)
        {
            if(string.IsNullOrWhiteSpace(searchText))
            {
                await LoadToolsLocalizations();
                return;
            }

            if(SelectedToolLocalizationSearchType == ToolLocalizationSearchType.Numer)
            {
                ToolsLocalizations.Clear();

                var toolsLocalizationsFromDb = await _toolLocalizationDataService.GetToolLocalizationsByNumber(searchText);
                ToolsLocalizations = toolsLocalizationsFromDb.ToObservableCollection();

                SelectedToolLocalization = ToolsLocalizations.FirstOrDefault();
            }

            if (SelectedToolLocalizationSearchType == ToolLocalizationSearchType.Opis)
            {
                ToolsLocalizations.Clear();

                var toolsLocalizationsFromDb = await _toolLocalizationDataService.GetToolLocalizationsByInfo(searchText);
                ToolsLocalizations = toolsLocalizationsFromDb.ToObservableCollection();

                SelectedToolLocalization = ToolsLocalizations.FirstOrDefault();
            }
        }

        private async void SortLocalizastonsBy(ToolLocalizationSortType toolLocalizationSortType)
        {
            if (toolLocalizationSortType == ToolLocalizationSortType.Domyślnie)
                ToolsLocalizations = ToolsLocalizations.OrderBy(x => x.ToolLocalizationId).ToObservableCollection();

            if(toolLocalizationSortType == ToolLocalizationSortType.Numer)
                ToolsLocalizations = ToolsLocalizations.OrderBy(x => x.ToolLocalizationNumber).ToObservableCollection();

            if (toolLocalizationSortType == ToolLocalizationSortType.Opis)
                ToolsLocalizations = ToolsLocalizations.OrderBy(x => x.ToolLocalizationDescription).ToObservableCollection();
        }

        private async void AddToolLocalization()
        {
            ViewMessenger.Default.Send<ToolLocalization>(null);
            ViewMessenger.Default.Send(_sentUser);

            var catalogViewService = new ViewDialogService<AddOrUpdateToolsLocalizationView>(new AddOrUpdateToolsLocalizationView(_sentUser));
            catalogViewService.ShowGenericWindow();

            await LoadDataAsync();
        }

        private async void UpdateToolLocalization()
        {
            ViewMessenger.Default.Send(SelectedToolLocalization);
            ViewMessenger.Default.Send(_sentUser);

            var catalogViewService = new ViewDialogService<AddOrUpdateToolsLocalizationView>(new AddOrUpdateToolsLocalizationView(_sentUser));
            catalogViewService.ShowGenericWindow();

            await LoadDataAsync();
        }

        private async void DeleteToolLocalization()
        {
            if (SelectedToolLocalization == null)
                return;

            try
            {
                var question = MessageBox.Show("Czy usunać lokalizację?", "Błąd", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (question == MessageBoxResult.No)
                    return;

                if (SelectedToolLocalization.Tools.Any())
                {
                    MessageBox.Show("Nie można usunąć lokalizacji ponieważ są do niej przypisane narzędzia.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await _toolLocalizationDataService.DeleteLocalization(SelectedToolLocalization);
                await _userLogDataService.AddUserLog(new UserLog($"Użytkownik usunął lokalizację narzędzi " +
                    $"{SelectedToolLocalization.ToolLocalizationNumber}.", _sentUser));

                await LoadDataAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private ICommand _addToolLocalizationCommand;
        public ICommand AddToolLocalizationCommand
        {
            get
            {
                if (_addToolLocalizationCommand == null)
                    _addToolLocalizationCommand = new CustomCommand((object o) => { AddToolLocalization(); }, (object o) => { return true; });

                return _addToolLocalizationCommand;
            }
            set { _addToolLocalizationCommand = value; }
        }

        private ICommand _updateToolLocalizationCommand;
        public ICommand UpdateToolLocalizationCommand
        {
            get
            {
                if (_updateToolLocalizationCommand == null)
                    _updateToolLocalizationCommand = new CustomCommand((object o) => { UpdateToolLocalization(); }, (object o) => { return true; });

                return _updateToolLocalizationCommand;
            }
            set { _updateToolLocalizationCommand = value; }
        }

        private ICommand _deleteToolLocalizationCommand;
        public ICommand DeleteToolLocalizationCommand
        {
            get
            {
                if (_deleteToolLocalizationCommand == null)
                    _deleteToolLocalizationCommand = new CustomCommand((object o) => { DeleteToolLocalization(); }, (object o) => { return true; });

                return _deleteToolLocalizationCommand;
            }
            set { _deleteToolLocalizationCommand = value; }
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

        private ObservableCollection<ToolLocalization> _toolsLocalizations;
        public ObservableCollection<ToolLocalization> ToolsLocalizations
        {
            get { return _toolsLocalizations; }
            set { _toolsLocalizations = value; OnPropertyChanged(nameof(ToolsLocalizations)); }
        }

        private ToolLocalization _selectedToolLocalization;
        public ToolLocalization SelectedToolLocalization
        {
            get { return _selectedToolLocalization; }
            set
            {
                _selectedToolLocalization = value;
                OnPropertyChanged(nameof(SelectedToolLocalization));
                Tools = SelectedToolLocalization?.Tools.ToObservableCollection() ?? new ObservableCollection<Tool>();
            }
        }

        private ObservableCollection<Tool> _tools;
        public ObservableCollection<Tool> Tools
        {
            get { return _tools; }
            set { _tools = value; OnPropertyChanged(nameof(Tools)); }
        }

        private List<ToolLocalizationSearchType> _toolLocalizationSearchTypes;
        public List<ToolLocalizationSearchType> ToolLocalizationSearchTypes
        {
            get { return _toolLocalizationSearchTypes; }
            set { _toolLocalizationSearchTypes = value; OnPropertyChanged(nameof(ToolLocalizationSearchTypes)); }
        }

        private ToolLocalizationSearchType _selectedToolLocalizationSearchType;
        public ToolLocalizationSearchType SelectedToolLocalizationSearchType
        {
            get { return _selectedToolLocalizationSearchType; }
            set { _selectedToolLocalizationSearchType = value; OnPropertyChanged(nameof(SelectedToolLocalizationSearchType)); }
        }

        private List<ToolLocalizationSortType> _toolLocalizationSortTypes;
        public List<ToolLocalizationSortType> ToolLocalizationSortTypes
        {
            get { return _toolLocalizationSortTypes; }
            set { _toolLocalizationSortTypes = value; OnPropertyChanged(nameof(ToolLocalizationSortTypes)); }
        }

        private ToolLocalizationSortType _selectedToolLocalizationSortType;
        public ToolLocalizationSortType SelectedToolLocalizationSortType
        {
            get { return _selectedToolLocalizationSortType; }
            set
            {
                _selectedToolLocalizationSortType = value;
                OnPropertyChanged(nameof(SelectedToolLocalizationSortType));
                SortLocalizastonsBy(SelectedToolLocalizationSortType);
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
                SearchLocalizationsBy(SearchText);
            }
        }
    }
}
