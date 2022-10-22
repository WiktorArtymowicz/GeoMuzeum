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

namespace GeoMuzeum.View.Views.ToolsStocktakingUserControl.AddOrUpdateToolStocktaking
{
    public class AddOrUpdateToolsStocktakingViewModel : BaseViewModel
    {
        private readonly IToolDataService _toolDataService;
        private readonly IToolLocalizationDataService _toolLocalizationDataService;
        private readonly IToolStocktakingDataService _toolStocktakingDataService;
        private readonly IUserLogDataService _userLogDataService;

        private ToolStocktaking _sentToolStocktaking;
        private User _sentUser;

        public Action CloseAction;

        protected AddOrUpdateToolsStocktakingViewModel()
        {

        }

        public AddOrUpdateToolsStocktakingViewModel(IToolDataService toolDataService, IToolLocalizationDataService toolLocalizationDataService, IToolStocktakingDataService toolStocktakingDataService, IUserLogDataService userLogDataService)
        {
            _toolDataService = toolDataService;
            _toolLocalizationDataService = toolLocalizationDataService;
            _toolStocktakingDataService = toolStocktakingDataService;
            _userLogDataService = userLogDataService;

            ViewMessenger.Default.Register<ToolStocktaking>(this, OnToolStocktakingRecived);
        }

        private void OnToolStocktakingRecived(ToolStocktaking toolStocktaking)
        {
            _sentToolStocktaking = toolStocktaking;
            _editStatusType = _sentToolStocktaking == null ? EditStatusType.Add : EditStatusType.Modify;
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
                var toolStocktakings = await _toolStocktakingDataService.GetAllToolsStocktakings();
                var toolIds = toolStocktakings.Select(x => x.Tool.ToolId).ToList();

                ToolStocktaking = _sentToolStocktaking == null ? new ToolStocktaking() : await _toolStocktakingDataService.FindToolStocktakingPositionById(_sentToolStocktaking);

                var tools = await _toolDataService.GetAllTools();

                Tools = EditStatusType == EditStatusType.Add ? tools.Where(x => !toolIds.Any(y => y == x.ToolId)).ToObservableCollection() : tools.ToObservableCollection();
                SelectedTool = ToolStocktaking.Tool == null ? Tools.FirstOrDefault() : Tools.FirstOrDefault(x => x.ToolId == ToolStocktaking.Tool.ToolId);

                var toolLocalizations = await _toolLocalizationDataService.GetAllToolLocalizations();
                ToolLocalizations = toolLocalizations.ToObservableCollection();
                SelectedToolLocalization = ToolStocktaking.Localization == null ? ToolLocalizations.FirstOrDefault() : ToolLocalizations.FirstOrDefault(x => x.ToolLocalizationId == ToolStocktaking.Localization.ToolLocalizationId);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private ToolStocktaking FillToolStocktakingToSave()
        {
            ToolStocktaking.Localization = SelectedToolLocalization;
            ToolStocktaking.Tool = SelectedTool;

            return ToolStocktaking;
        }

        private async void AddOrUpdateToolStocktaking(ToolStocktaking toolStocktaking)
        {
            try
            {
                if (_editStatusType == EditStatusType.Add)
                {
                    await _toolStocktakingDataService.AddToolStocktakingPosition(toolStocktaking);
                    await _userLogDataService.AddUserLog(new UserLog($"Użytkownik dodał pozycję remanentu narzędzi {toolStocktaking.Tool.ToolName}.", _sentUser));

                }
                else
                {
                    await _toolStocktakingDataService.UpdateToolStocktakingPosition(toolStocktaking);
                    await _userLogDataService.AddUserLog(new UserLog($"Użytkownik edytował pozycję remanentu narzędzi {toolStocktaking.Tool.ToolName}.", _sentUser));
                }

                CloseWindow();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
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
                    _saveCommand = new CustomCommand((object o) => { AddOrUpdateToolStocktaking(FillToolStocktakingToSave()); }, (object o) => { return true; });

                return _saveCommand;
            }
            set { _saveCommand = value; }
        }

        private ToolStocktaking _toolStocktaking;
        public ToolStocktaking ToolStocktaking
        {
            get { return _toolStocktaking; }
            set { _toolStocktaking = value; OnPropertyChanged(nameof(ToolStocktaking)); }
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

        private ObservableCollection<ToolLocalization> _toolLocalizations;
        public ObservableCollection<ToolLocalization> ToolLocalizations
        {
            get { return _toolLocalizations; }
            set { _toolLocalizations = value; OnPropertyChanged(nameof(ToolLocalizations)); }
        }

        private ToolLocalization _selectedToolLocalization;
        public ToolLocalization SelectedToolLocalization
        {
            get { return _selectedToolLocalization; }
            set { _selectedToolLocalization = value; OnPropertyChanged(nameof(SelectedToolLocalization)); }
        }

        private EditStatusType _editStatusType;
        public EditStatusType EditStatusType
        {
            get { return _editStatusType; }
            set { _editStatusType = value; OnPropertyChanged(nameof(EditStatusType)); }
        }
    }
}
