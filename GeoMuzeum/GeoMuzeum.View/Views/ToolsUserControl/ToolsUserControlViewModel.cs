using GeoMuzeum.DataService;
using GeoMuzeum.Model;
using GeoMuzeum.View.ProjectHelpers;
using GeoMuzeum.View.Views.ToolsUserControl.AddOrUpdateTool;
using GeoMuzeum.View.ViewServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GeoMuzeum.View.Views.ToolsUserControl
{
    public class ToolsUserControlViewModel : BaseViewModel
    {
        private readonly IToolDataService _toolDataService;
        private readonly IUserLogDataService _userLogDataService;
        private readonly ISettingsDataService _settingsDataService;

        private User _sentUser;
        private Settings _settings;

        protected ToolsUserControlViewModel()
        {

        }

        public ToolsUserControlViewModel(IToolDataService toolDataService, IUserLogDataService userLogDataService, ISettingsDataService settingsDataService)
        {
            _toolDataService = toolDataService;
            _userLogDataService = userLogDataService;
            _settingsDataService = settingsDataService;

            Tools = new ObservableCollection<Tool>();
            ToolSearchTypes = new List<ToolSearchType>();
            ToolSortTypes = new List<ToolSortType>();
        }

        private async Task LoadSeettings()
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
                IsToolStocktakingActive = _settings.IsToolStocktaking;
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
                await LoadSeettings();
                await LoadTools();
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
            ToolSortTypes.Clear();

            ToolSortTypes = Enum.GetValues(typeof(ToolSortType)).Cast<ToolSortType>().ToList();
            SelectedToolSortType = ToolSortTypes.FirstOrDefault();
        }

        private void LoadSearchTypes()
        {
            ToolSearchTypes.Clear();

            ToolSearchTypes = Enum.GetValues(typeof(ToolSearchType)).Cast<ToolSearchType>().ToList();
            SelectedToolSearchType = ToolSearchTypes.FirstOrDefault();
        }

        private async Task LoadTools()
        {
            Tools.Clear();

            var toolsFromDb = await _toolDataService.GetAllTools();
            Tools = toolsFromDb.ToObservableCollection();

            SelectedTool = Tools.FirstOrDefault();
        }

        private async void SearchToolsBy(string searchText)
        {
            if(string.IsNullOrWhiteSpace(searchText))
            {
                await LoadTools();
                return;
            }

            if(SelectedToolSearchType == ToolSearchType.Nazwa)
            {
                Tools.Clear();

                var tools = await _toolDataService.GetToolsByName(searchText);
                Tools = tools.ToObservableCollection();

                SelectedTool = Tools.FirstOrDefault();
            }

            if(SelectedToolSearchType == ToolSearchType.Lokalizacja)
            {
                Tools.Clear();

                var tools = await _toolDataService.GetToolsByLocalization(searchText);
                Tools = tools.ToObservableCollection();

                SelectedTool = Tools.FirstOrDefault();
            }

            if(SelectedToolSearchType == ToolSearchType.Opis)
            {
                Tools.Clear();

                var tools = await _toolDataService.GetToolsByInfo(searchText);
                Tools = tools.ToObservableCollection();

                SelectedTool = Tools.FirstOrDefault();
            }
        }

        private async void SortToolsBy(ToolSortType toolSortType)
        {
            if (toolSortType == ToolSortType.Domyślnie)
                Tools = Tools.OrderBy(x => x.ToolId).ToObservableCollection();

            if(toolSortType == ToolSortType.Lokalizacja)
                Tools = Tools.OrderBy(x => x.Localization.ToolLocalizationNumber).ToObservableCollection();

            if(toolSortType == ToolSortType.Nazwa)
                Tools = Tools.OrderBy(x => x.Localization.ToolLocalizationNumber).ToObservableCollection();

            if (toolSortType == ToolSortType.Opis)
                Tools = Tools.OrderBy(x => x.ToolDescription).ToObservableCollection();
        }

        private async void AddTool()
        {
            ViewMessenger.Default.Send<Tool>(null);

            var catalogViewService = new ViewDialogService<AddOrUpdateToolView>(new AddOrUpdateToolView(_sentUser));
            catalogViewService.ShowGenericWindow();

            await LoadDataAsync();
        }

        private async void UpdateTool()
        {
            ViewMessenger.Default.Send(SelectedTool);

            var catalogViewService = new ViewDialogService<AddOrUpdateToolView>(new AddOrUpdateToolView(_sentUser));
            catalogViewService.ShowGenericWindow();

            await LoadDataAsync();
        }

        private async void DeleteTool()
        {
            
            try
            {
                if (SelectedTool == null)
                    return;

                var question = MessageBox.Show("Czy usunać eksponat?", "Błąd", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (question == MessageBoxResult.No)
                    return;

                await _toolDataService.DeleteTool(SelectedTool);
                await _userLogDataService.AddUserLog(new UserLog($"Użytkownik usunął narzędzie {SelectedTool.ToolName}.", _sentUser));

                await LoadDataAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private ICommand _addToolCommand;
        public ICommand AddToolCommand
        {
            get
            {
                if (_addToolCommand == null)
                    _addToolCommand = new CustomCommand( (object o) => { AddTool(); }, (object o) => { return true; });

                return _addToolCommand;
            }
            set { _addToolCommand = value; }
        }

        private ICommand _updateToolCommand;
        public ICommand UpdateToolCommand
        {
            get
            {
                if (_updateToolCommand == null)
                    _updateToolCommand = new CustomCommand((object o) => { UpdateTool(); }, (object o) => { return !IsToolStocktakingActive; });

                return _updateToolCommand;
            }
            set { _updateToolCommand = value; }
        }

        private ICommand _deleteToolCommand;
        public ICommand DeleteToolCommand
        {
            get
            {
                if (_deleteToolCommand == null)
                    _deleteToolCommand = new CustomCommand((object o) => { DeleteTool(); }, (object o) => { return !IsToolStocktakingActive; });

                return _deleteToolCommand;
            }
            set { _deleteToolCommand = value; }
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

        private ObservableCollection<Tool> _tools;
        public ObservableCollection<Tool> Tools
        {
            get { return _tools; }
            set { _tools = value; OnPropertyChanged(nameof(Tools)); }
        }

        private Tool _selectedTool;
        public Tool SelectedTool
        {
            get { return _selectedTool; }
            set { _selectedTool = value; OnPropertyChanged(nameof(SelectedTool)); }
        }

        private List<ToolSearchType> _toolSearchTypes;
        public List<ToolSearchType> ToolSearchTypes
        {
            get { return _toolSearchTypes; }
            set { _toolSearchTypes = value; OnPropertyChanged(nameof(ToolSearchTypes)); }
        }

        private ToolSearchType _selectedToolSearchType;
        public ToolSearchType SelectedToolSearchType
        {
            get { return _selectedToolSearchType; }
            set { _selectedToolSearchType = value; OnPropertyChanged(nameof(SelectedToolSearchType)); }
        }

        private List<ToolSortType> _toolSortTypes;
        public List<ToolSortType> ToolSortTypes
        {
            get { return _toolSortTypes; }
            set { _toolSortTypes = value; OnPropertyChanged(nameof(ToolSortTypes)); }
        }

        private ToolSortType _selectedToolSortType;
        public ToolSortType SelectedToolSortType
        {
            get { return _selectedToolSortType; }
            set
            {
                _selectedToolSortType = value;
                OnPropertyChanged(nameof(SelectedToolSortType));
                SortToolsBy(SelectedToolSortType);
            }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SelectedToolSortType));
                SearchToolsBy(SearchText);
            }
        }

        private bool _isToolStocktakingActive;
        public bool IsToolStocktakingActive
        {
            get { return _isToolStocktakingActive; }
            set { _isToolStocktakingActive = value; OnPropertyChanged(nameof(IsToolStocktakingActive)); }
        }
    }
}
