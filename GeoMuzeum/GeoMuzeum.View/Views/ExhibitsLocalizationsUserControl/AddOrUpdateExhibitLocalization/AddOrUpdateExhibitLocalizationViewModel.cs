using GeoMuzeum.DataService;
using GeoMuzeum.Model;
using GeoMuzeum.View.Enums;
using GeoMuzeum.View.ProjectHelpers;
using GeoMuzeum.View.ViewServices;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GeoMuzeum.View.Views.ExhibitsLocalizationsUserControl.AddOrUpdateExhibitLocalization
{
    public class AddOrUpdateExhibitLocalizationViewModel : BaseViewModel
    {
        private readonly IExhibitLocalizationDataService _exhibitLocalizationDataService;
        private readonly IUserLogDataService _userLogDataService;

        private ExhibitLocalization _sentExhibitLocalization;
        private User _sentUser;
        private EditStatusType _editStatusType;

        public Action CloseAction { get; set; }

        public AddOrUpdateExhibitLocalizationViewModel()
        {

        }

        public AddOrUpdateExhibitLocalizationViewModel(IExhibitLocalizationDataService exhibitLocalizationDataService, IUserLogDataService userLogDataService)
        {
            _exhibitLocalizationDataService = exhibitLocalizationDataService;
            _userLogDataService = userLogDataService;

            ViewMessenger.Default.Register<ExhibitLocalization>(this, OnExhibitLocalizationRecived);
        }

        private void OnExhibitLocalizationRecived(ExhibitLocalization exhibitLocalization)
        {
            _sentExhibitLocalization = exhibitLocalization;
            _editStatusType = _sentExhibitLocalization != null ? EditStatusType.Modify : EditStatusType.Add;
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
                var exhibitLocalization = _sentExhibitLocalization == null ? new ExhibitLocalization()
                : await _exhibitLocalizationDataService.GetExhibitLocalizationById(_sentExhibitLocalization);

                ExhibitLocalization = exhibitLocalization;
                ExhibitLocalizationNumber = exhibitLocalization.ExhibitLocalizationNumber;
                ExhibitLocalizationDescription = exhibitLocalization.ExhibitLocalizationDescription;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private async void AddOrUpdateExhibitLocalization(ExhibitLocalization exhibitLocalization)
        {
            try
            {
                if (ValidateExhibitLocalization(exhibitLocalization) == false)
                    return;

                if (_editStatusType == EditStatusType.Add)
                {
                    await _exhibitLocalizationDataService.SaveNewExhibitLocalization(exhibitLocalization);
                    await _userLogDataService.AddUserLog(new UserLog($"Użytkownik dodał lokalizację eksponatów {exhibitLocalization.ExhibitLocalizationNumber}.", _sentUser));
                }

                else
                {
                    await _exhibitLocalizationDataService.UdateExhibitLocalization(exhibitLocalization);
                    await _userLogDataService.AddUserLog(new UserLog($"Użytkownik edytował lokalizację eksponatów {exhibitLocalization.ExhibitLocalizationNumber}.", _sentUser));
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            finally
            {
                CloseWindow();
            }
        }

        private bool ValidateExhibitLocalization(ExhibitLocalization exhibitLocalization)
        {
            if (string.IsNullOrWhiteSpace(exhibitLocalization.ExhibitLocalizationNumber))
            {
                MessageBox.Show($"Proszę uzupełnić numer lokalizacji.", "Brak wprowadzonych danych.", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private ExhibitLocalization FillExhibitLocalizationToSave()
        {
            ExhibitLocalization.ExhibitLocalizationDescription = ExhibitLocalizationDescription;
            ExhibitLocalization.ExhibitLocalizationNumber = ExhibitLocalizationNumber;

            return ExhibitLocalization;
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
                    _saveCommand = new CustomCommand((object o) => { AddOrUpdateExhibitLocalization(FillExhibitLocalizationToSave()); }, (object o) => { return true; });

                return _saveCommand;
            }
            set { _saveCommand = value; }
        }

        private ExhibitLocalization _exhibitLocalization;
        public ExhibitLocalization ExhibitLocalization
        {
            get { return _exhibitLocalization; }
            set { _exhibitLocalization = value; OnPropertyChanged(nameof(ExhibitLocalization)); }
        }

        private string _exhibitLocalizationNumber;
        public string ExhibitLocalizationNumber
        {
            get { return _exhibitLocalizationNumber; }
            set { _exhibitLocalizationNumber = value; OnPropertyChanged(nameof(ExhibitLocalizationNumber)); }
        }

        private string _exhibitLocalizationDescription;
        public string ExhibitLocalizationDescription
        {
            get { return _exhibitLocalizationDescription; }
            set { _exhibitLocalizationDescription = value; OnPropertyChanged(nameof(ExhibitLocalizationDescription)); }
        }
    }
}
