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

namespace GeoMuzeum.View.Views.ToolsUserControl.AddOrUpdateTool
{
    public class AddOrUpdateToolViewModel : BaseViewModel
    {
        private readonly IToolDataService _toolDataService;
        private readonly IToolLocalizationDataService _toolLocalizationDataService;
        private readonly IUserLogDataService _userLogDataService;

        private Tool _sentTool;
        private User _sentUser;
        private EditStatusType _editStatusType;


        protected AddOrUpdateToolViewModel()
        {

        }

        public AddOrUpdateToolViewModel(IToolDataService toolDataService, IToolLocalizationDataService toolLocalizationDataService, IUserLogDataService userLogDataService)
        {
            _toolDataService = toolDataService;
            _toolLocalizationDataService = toolLocalizationDataService;
            _userLogDataService = userLogDataService;

            ToolLocalizations = new ObservableCollection<ToolLocalization>();

            ViewMessenger.Default.Register<Tool>(this, OnToolRecived);
        }

        private void OnToolRecived(Tool tool)
        {
            _sentTool = tool;
            _editStatusType = _sentTool == null ? EditStatusType.Add : EditStatusType.Modify;
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


        public Action CloseAction { get; set; }

        public async override Task LoadDataAsync()
        {
            try
            {
                Tool = _sentTool == null ? new Tool() : await _toolDataService.GetToolById(_sentTool);

                ToolName = Tool.ToolName;
                ToolDescription = Tool.ToolDescription;

                var toolLocalizations = await _toolLocalizationDataService.GetAllToolLocalizations();
                ToolLocalizations = toolLocalizations.ToObservableCollection();

                SelectedToolLocalization = Tool.Localization == null ? ToolLocalizations.FirstOrDefault() : ToolLocalizations.FirstOrDefault(x => x.ToolLocalizationId == Tool.Localization.ToolLocalizationId);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private Tool FillToolToSave()
        {
            Tool.ToolName = ToolName;
            Tool.ToolDescription = ToolDescription;
            Tool.Localization = SelectedToolLocalization;

            return Tool;
        }

        private bool ValidateTool(Tool tool)
        {
            if(string.IsNullOrWhiteSpace(tool.ToolName))
            {
                MessageBox.Show($"Proszę uzupełnić numer nazwę narzędzia.", "Brak wprowadzonych danych.", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private async void AddOrUpdateTool(Tool tool)
        {
            try
            {
                if (ValidateTool(tool) == false)
                    return;

                if (_editStatusType == EditStatusType.Add)
                {
                    await _toolDataService.AddNewTool(tool);
                    await _userLogDataService.AddUserLog(new UserLog($"Użytkownik dodał narzędzie {tool.ToolName}.", _sentUser));
                }
                else
                {
                    await _toolDataService.UpdateTool(tool);
                    await _userLogDataService.AddUserLog(new UserLog($"Użytkownik edytował narzędzie {tool.ToolName}.", _sentUser));
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
                    _saveCommand = new CustomCommand((object o) => { AddOrUpdateTool(FillToolToSave()); }, (object o) => { return true; });

                return _saveCommand;
            }
            set { _saveCommand = value; }
        }

        private Tool _tool;
        public Tool Tool
        {
            get { return _tool; }
            set { _tool = value; OnPropertyChanged(nameof(Tool)); }
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

        private string _toolName;
        public string ToolName
        {
            get { return _toolName; }
            set { _toolName = value; OnPropertyChanged(nameof(ToolName)); }
        }

        private string _toolDescription;
        public string ToolDescription
        {
            get { return _toolDescription; }
            set { _toolDescription = value; OnPropertyChanged(nameof(ToolDescription)); }
        }
    }
}
