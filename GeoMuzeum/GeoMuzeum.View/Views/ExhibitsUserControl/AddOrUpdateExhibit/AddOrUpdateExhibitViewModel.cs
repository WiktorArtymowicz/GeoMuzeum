using GeoMuzeum.DataService;
using GeoMuzeum.Model;
using GeoMuzeum.Model.Enums;
using GeoMuzeum.View.Enums;
using GeoMuzeum.View.ProjectHelpers;
using GeoMuzeum.View.ViewServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GeoMuzeum.View.Views.ExhibitsUserControl.AddOrUpdateExhibit
{
    public class AddOrUpdateExhibitViewModel : BaseViewModel
    {
        private readonly IExhibitDataService _exhibitDataService;
        private readonly ICatalogDataService _catalogDataService;
        private readonly IExhibitLocalizationDataService _exhibitLocalizationDataService;
        private readonly IUserLogDataService _userLogDataService;

        private Exhibit _sentExhibit;
        private User _sentUser;
        private EditStatusType _editStatusType;

        protected AddOrUpdateExhibitViewModel()
        {

        }

        public AddOrUpdateExhibitViewModel(IExhibitDataService exhibitDataService, ICatalogDataService catalogDataService, IExhibitLocalizationDataService exhibitLocalizationDataService, IUserLogDataService userLogDataService)
        {
            _exhibitDataService = exhibitDataService;
            _catalogDataService = catalogDataService;
            _exhibitLocalizationDataService = exhibitLocalizationDataService;
            _userLogDataService = userLogDataService;

            Catalogs = new ObservableCollection<Catalog>();
            ExhibitLocalizations = new ObservableCollection<ExhibitLocalization>();
            ExhibitTypes = new List<ExhibitType>();

            ViewMessenger.Default.Register<Exhibit>(this, OnExhibitRecived);
        }

        private void OnExhibitRecived(Exhibit exhibit)
        {
            _sentExhibit = exhibit;
            _editStatusType = _sentExhibit == null ? EditStatusType.Add : EditStatusType.Modify;
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
                Exhibit = _sentExhibit == null ? new Exhibit() : await _exhibitDataService.GetExhibitById(_sentExhibit);

                ExhibitName = Exhibit?.ExhibitName;
                ExhibitDescprition = Exhibit?.ExhibitDescription;

                ExhibitTypes = Enum.GetValues(typeof(ExhibitType)).Cast<ExhibitType>().ToList();
                SelectedExhibitType = Exhibit == null ? ExhibitTypes.FirstOrDefault() : ExhibitTypes.FirstOrDefault(x => x.Equals(Exhibit.ExhibitType));

                var catalogs = await _catalogDataService.GetAllCatalogs();
                Catalogs = catalogs.ToObservableCollection();
                SelectedCatalog = Exhibit.Catalog == null ? Catalogs.FirstOrDefault() : Catalogs.FirstOrDefault(x => x.CatalogId == Exhibit.Catalog.CatalogId);

                var exhibitLocalizations = await _exhibitLocalizationDataService.GetExhibitLocalizations();
                ExhibitLocalizations = exhibitLocalizations.ToObservableCollection();
                SelectedExhibitLocalization = Exhibit.Localization == null ? ExhibitLocalizations.FirstOrDefault() : ExhibitLocalizations.FirstOrDefault(x => x.ExhibitLocalizationId == Exhibit.Localization.ExhibitLocalizationId);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private Exhibit FillExhibitToSave()
        {
            Exhibit.ExhibitDescription = ExhibitDescprition;
            Exhibit.ExhibitName = ExhibitName;
            Exhibit.ExhibitType = SelectedExhibitType;
            Exhibit.Localization = SelectedExhibitLocalization;
            Exhibit.Catalog = SelectedCatalog;

            return Exhibit;
        }

        private async void AddOrUpdateExhibit(Exhibit exhibit)
        {
            try
            {
                if (ValidateExhibit(exhibit) == false)
                    return;

                if (_editStatusType == EditStatusType.Add)
                {
                    await _exhibitDataService.SaveNewExhibit(exhibit);
                    await _userLogDataService.AddUserLog(new UserLog($"Użytkownik dodał eksponat {exhibit.ExhibitName}.", _sentUser));
                }

                else
                {
                    await _exhibitDataService.UpdateExhibit(exhibit);
                    await _userLogDataService.AddUserLog(new UserLog($"Użytkownik edytował eksponat {exhibit.ExhibitName}.", _sentUser));
                }

                CloseWindow();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private bool ValidateExhibit(Exhibit exhibit)
        {
            if (string.IsNullOrWhiteSpace(exhibit.ExhibitName))
            {
                MessageBox.Show($"Proszę uzupełnić numer nazwę eksponatu.", "Brak wprowadzonych danych.", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
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
                    _saveCommand = new CustomCommand((object o) => { AddOrUpdateExhibit(FillExhibitToSave()); }, (object o) => { return true; });

                return _saveCommand;
            }
            set { _saveCommand = value; }
        }

        public Action CloseAction { get; set; }

        private Exhibit _exhibit;
        public Exhibit Exhibit
        {
            get { return _exhibit; }
            set { _exhibit = value; }
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

        private List<ExhibitType> _exhibitTypes;
        public List<ExhibitType> ExhibitTypes
        {
            get { return _exhibitTypes; }
            set { _exhibitTypes = value; OnPropertyChanged(nameof(ExhibitTypes)); }
        }

        private ExhibitType _selectedExhibitType;
        public ExhibitType SelectedExhibitType
        {
            get { return _selectedExhibitType; }
            set { _selectedExhibitType = value; OnPropertyChanged(nameof(SelectedExhibitType)); }
        }

        private string _exhibitName;
        public string ExhibitName
        {
            get { return _exhibitName; }
            set { _exhibitName = value; OnPropertyChanged(nameof(ExhibitName)); }
        }

        private string _exhibitDescription;
        public string ExhibitDescprition
        {
            get { return _exhibitDescription; }
            set { _exhibitDescription = value; OnPropertyChanged(nameof(ExhibitDescprition)); }
        }
    }
}
