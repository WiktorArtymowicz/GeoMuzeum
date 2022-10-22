using GeoMuzeum.DataService;
using GeoMuzeum.Model;
using GeoMuzeum.View.Enums;
using GeoMuzeum.View.ProjectHelpers;
using GeoMuzeum.View.ViewServices;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GeoMuzeum.View.Views.ToolsLocalizationsUserControl.AddOrUpdateToolsLocalization
{
    public class AddOrUpdateToolsLocalizationViewModel : BaseViewModel
    {
        private readonly IToolLocalizationDataService _toolLocalizationDataService;
        private readonly IUserLogDataService _userLogDataService;

        private EditStatusType _editStatusType;
        private ToolLocalization _sentToolLocalization;
        private User _sentUser;

        public Action CloseAction { get; set; }

        public AddOrUpdateToolsLocalizationViewModel(IToolLocalizationDataService toolLocalizationDataService, IUserLogDataService userLogDataService)
        {
            _toolLocalizationDataService = toolLocalizationDataService;
            _userLogDataService = userLogDataService;

            ViewMessenger.Default.Register<ToolLocalization>(this, OnToolLocalizationRecived);
        }

        private void OnToolLocalizationRecived(ToolLocalization toolLocalization)
        {
            _sentToolLocalization = toolLocalization;
            _editStatusType = _sentToolLocalization == null ? EditStatusType.Add : EditStatusType.Modify;
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


        public override async Task LoadDataAsync()
        {
            try
            {
                ToolLocalization = _sentToolLocalization == null ? new ToolLocalization() : await _toolLocalizationDataService.GetToolLocalizationById(_sentToolLocalization);

                ToolLocalizationNumber = ToolLocalization.ToolLocalizationNumber;
                ToolLocalizationDescription = ToolLocalization.ToolLocalizationDescription;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private ToolLocalization FillToolLocalizationToSave()
        {
            ToolLocalization.ToolLocalizationNumber = ToolLocalizationNumber;
            ToolLocalization.ToolLocalizationDescription = ToolLocalizationDescription;

            return ToolLocalization;
        }

        private bool ValidateToolLocalization(ToolLocalization toolLocalization)
        {
            if (string.IsNullOrWhiteSpace(toolLocalization.ToolLocalizationNumber))
            {
                MessageBox.Show($"Proszę uzupełnić numer lokalizacji.", "Brak wprowadzonych danych.", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private async void AddOrUpdateToolLocalization(ToolLocalization toolLocalization)
        {
            try
            {
                if (ValidateToolLocalization(toolLocalization) == false)
                    return;

                if (_editStatusType == EditStatusType.Add)
                {
                    await _toolLocalizationDataService.AddNewLocalization(toolLocalization);
                    await _userLogDataService.AddUserLog(new UserLog($"Użytkownik dodał lokalizację narzędzi {toolLocalization.ToolLocalizationNumber}.", _sentUser));
                }  
                else
                {
                    await _toolLocalizationDataService.UpdateLocalization(toolLocalization);
                    await _userLogDataService.AddUserLog(new UserLog($"Użytkownik edytował lokalizację narzędzi {toolLocalization.ToolLocalizationNumber}.", _sentUser));
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
                    _saveCommand = new CustomCommand((object o) => { AddOrUpdateToolLocalization(FillToolLocalizationToSave()); }, (object o) => { return true; });

                return _saveCommand;
            }
            set { _saveCommand = value; }
        }

        private ToolLocalization _toolLocalization;
        public ToolLocalization ToolLocalization
        {
            get { return _toolLocalization; }
            set { _toolLocalization = value; OnPropertyChanged(nameof(ToolLocalization)); }
        }

        private string _toolLocalizationNumber;
        public string ToolLocalizationNumber
        {
            get { return _toolLocalizationNumber; }
            set { _toolLocalizationNumber = value; OnPropertyChanged(nameof(ToolLocalizationNumber)); }
        }

        private string _toolLocalizationDescription;
        public string ToolLocalizationDescription
        {
            get { return _toolLocalizationDescription; }
            set { _toolLocalizationDescription = value; OnPropertyChanged(nameof(ToolLocalizationDescription)); }
        }
    }
}
