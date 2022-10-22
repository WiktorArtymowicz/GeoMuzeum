using GeoMuzeum.DataService;
using GeoMuzeum.Model;
using GeoMuzeum.View.ProjectHelpers;
using GeoMuzeum.View.Views.ExhibitsLocalizationsUserControl.AddOrUpdateExhibitLocalization;
using GeoMuzeum.View.ViewServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GeoMuzeum.View.Views.ExhibitsLocalizationsUserControl
{
    public class ExhibitsLocalizationsUserControlViewModel : BaseViewModel
    {
        private IExhibitLocalizationDataService _exhibitLocalizationDataService;
        private IExhibitDataService _exhibitDataService;
        private readonly IUserLogDataService _userLogDataService;

        private User _sentUser;

        protected ExhibitsLocalizationsUserControlViewModel()
        {

        }

        public ExhibitsLocalizationsUserControlViewModel(IExhibitLocalizationDataService exhibitLocalizationDataService, IExhibitDataService exhibitDataService, IUserLogDataService userLogDataService)
        {
            _exhibitLocalizationDataService = exhibitLocalizationDataService;
            _exhibitDataService = exhibitDataService;
            _userLogDataService = userLogDataService;

            ExhibitLocalizations = new ObservableCollection<ExhibitLocalization>();
            ExhibitsLocalizationSortTypes = new List<ExhibitLocalizationSortType>();
            ExhibitsLocalizationSerarchTypes = new List<ExhibitLocalizationSearchType>();
        }

        public override async Task LoadDataAsync()
        {
            try
            {
                await LoadExhibitLocalizations();
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

        public override void SetUser(User user)
        {
            if (user == null)
            {
                MessageBox.Show("Błąd przy przekazywaniu użytkownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _sentUser = user;
        }

        private void LoadSortTypes()
        {
            ExhibitsLocalizationSortTypes.Clear();

            ExhibitsLocalizationSortTypes = Enum.GetValues(typeof(ExhibitLocalizationSortType)).Cast<ExhibitLocalizationSortType>().ToList();
            SelectedExhibitsLocalizationSortType = ExhibitsLocalizationSortTypes.FirstOrDefault();
        }

        private void LoadSearchTypes()
        {
            ExhibitsLocalizationSerarchTypes.Clear();

            ExhibitsLocalizationSerarchTypes = Enum.GetValues(typeof(ExhibitLocalizationSearchType)).Cast<ExhibitLocalizationSearchType>().ToList();
            SelectedExhibitsLocalizationSearchType = ExhibitsLocalizationSerarchTypes.FirstOrDefault();
        }

        private async Task LoadExhibitLocalizations()
        {
            ExhibitLocalizations.Clear();

            var localizationsFromDb = await _exhibitLocalizationDataService.GetExhibitLocalizations();
            ExhibitLocalizations = localizationsFromDb.ToObservableCollection();

            SelectedExhibitLocalization = ExhibitLocalizations.FirstOrDefault();
        }

        private async void LoadExhibits(ExhibitLocalization exhibitLocalization)
        {
            if (exhibitLocalization == null)
                return;

            var localizations = await _exhibitDataService.GetAllExhibitsByLocalizationAsync(exhibitLocalization.ExhibitLocalizationNumber);
            ExhibitsOnLocalization = localizations.ToObservableCollection();
        }

        private async void SearchLocalizationsBy(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                await LoadExhibitLocalizations();
                return;
            }

            if (SelectedExhibitsLocalizationSearchType == ExhibitLocalizationSearchType.Numer)
            {
                ExhibitLocalizations.Clear();

                var localizations = await _exhibitLocalizationDataService.GetExhibitLocalizationsByName(searchText);
                ExhibitLocalizations = localizations.ToObservableCollection();

                SelectedExhibitLocalization = ExhibitLocalizations.FirstOrDefault();
            }

            if (SelectedExhibitsLocalizationSearchType == ExhibitLocalizationSearchType.Opis)
            {
                ExhibitLocalizations.Clear();

                var localizations = await _exhibitLocalizationDataService.GetExhibitLocalizationsByInfo(searchText);
                ExhibitLocalizations = localizations.ToObservableCollection();

                SelectedExhibitLocalization = ExhibitLocalizations.FirstOrDefault();
            }
        }

        private async void SortLocalizationsBy(ExhibitLocalizationSortType exhibitLocalizationSortType)
        {
            if (exhibitLocalizationSortType == ExhibitLocalizationSortType.Domyślnie)
                ExhibitLocalizations = ExhibitLocalizations.OrderBy(x => x.ExhibitLocalizationId).ToObservableCollection();

            if (exhibitLocalizationSortType == ExhibitLocalizationSortType.Numer)
                ExhibitLocalizations = ExhibitLocalizations.OrderBy(x => x.ExhibitLocalizationNumber).ToObservableCollection();

            if (exhibitLocalizationSortType == ExhibitLocalizationSortType.Opis)
                ExhibitLocalizations = ExhibitLocalizations.OrderBy(x => x.ExhibitLocalizationDescription).ToObservableCollection();
        }

        private async void AddExhibitLocalization()
        {
            ViewMessenger.Default.Send<ExhibitLocalization>(null);

            var catalogViewService = new ViewDialogService<AddOrUpdateExhibitLocalizationView>(new AddOrUpdateExhibitLocalizationView(_sentUser));
            catalogViewService.ShowGenericWindow();

            await LoadDataAsync();
        }

        private async void UpdateExhibitLocalization()
        {
            if (SelectedExhibitLocalization == null)
                return;

            ViewMessenger.Default.Send(SelectedExhibitLocalization);

            var catalogViewService = new ViewDialogService<AddOrUpdateExhibitLocalizationView>(new AddOrUpdateExhibitLocalizationView(_sentUser));
            catalogViewService.ShowGenericWindow();

            await LoadDataAsync();
        }

        private async void DeleteExhibitLocalization()
        {
            try
            {
                if (SelectedExhibitLocalization == null)
                    return;

                var question = MessageBox.Show("Czy usunać lokalizację?", "Błąd", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (question == MessageBoxResult.No)
                    return;

                if (SelectedExhibitLocalization.Exhibits.Any())
                {
                    MessageBox.Show("Nie można usunąć lokalizacji ponieważ są do niej przypisane eksponaty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await _exhibitLocalizationDataService.DeleteExhibitLocalization(SelectedExhibitLocalization);
                await _userLogDataService.AddUserLog(new UserLog($"Użytkownik usunął lokalizację eksponatów {SelectedExhibitLocalization.ExhibitLocalizationNumber}.", _sentUser));
                await LoadDataAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private ICommand _addExhibitLocalizationCommand;
        public ICommand AddExhibitLocalizationCommand
        {
            get
            {
                if (_addExhibitLocalizationCommand == null)
                    _addExhibitLocalizationCommand = new CustomCommand((object o) => { AddExhibitLocalization(); }, (object o) => { return true; });

                return _addExhibitLocalizationCommand;
            }
            set { _addExhibitLocalizationCommand = value; }
        }

        private ICommand _updateExhibitLocalizationCommand;
        public ICommand UpdateExhibitLocalizationCommand
        {
            get
            {
                if (_updateExhibitLocalizationCommand == null)
                    _updateExhibitLocalizationCommand = new CustomCommand((object o) => { UpdateExhibitLocalization(); }, (object o) => { return true; });

                return _updateExhibitLocalizationCommand;
            }
            set { _updateExhibitLocalizationCommand = value; }
        }

        private ICommand _deleteExhibitLocalizationCommand;
        public ICommand DeleteExhibitLocalizationCommand
        {
            get
            {
                if (_deleteExhibitLocalizationCommand == null)
                    _deleteExhibitLocalizationCommand = new CustomCommand((object o) => { DeleteExhibitLocalization(); }, (object o) => { return true; });

                return _deleteExhibitLocalizationCommand;
            }
            set { _deleteExhibitLocalizationCommand = value; }
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

        private ObservableCollection<ExhibitLocalization> _exhibitLocalizations;
        public ObservableCollection<ExhibitLocalization> ExhibitLocalizations
        {
            get { return _exhibitLocalizations; }
            set { _exhibitLocalizations = value; OnPropertyChanged(nameof(ExhibitLocalizations)); }
        }

        private ObservableCollection<Exhibit> _exhibistOnLocalization;
        public ObservableCollection<Exhibit> ExhibitsOnLocalization
        {
            get { return _exhibistOnLocalization; }
            set { _exhibistOnLocalization = value; OnPropertyChanged(nameof(ExhibitsOnLocalization)); }
        }

        private ExhibitLocalization _selectedExhibitLocalization;
        public ExhibitLocalization SelectedExhibitLocalization
        {
            get { return _selectedExhibitLocalization; }
            set
            {
                _selectedExhibitLocalization = value;
                OnPropertyChanged(nameof(SelectedExhibitLocalization));
                LoadExhibits(SelectedExhibitLocalization);
            }
        }

        private List<ExhibitLocalizationSearchType> _exhibitsLocalizationSerarchTypes;
        public List<ExhibitLocalizationSearchType> ExhibitsLocalizationSerarchTypes
        {
            get { return _exhibitsLocalizationSerarchTypes; }
            set { _exhibitsLocalizationSerarchTypes = value; OnPropertyChanged(nameof(ExhibitsLocalizationSerarchTypes)); }
        }

        private ExhibitLocalizationSearchType _selectedExhibitsLocalizationSearchType;
        public ExhibitLocalizationSearchType SelectedExhibitsLocalizationSearchType
        {
            get { return _selectedExhibitsLocalizationSearchType; }
            set { _selectedExhibitsLocalizationSearchType = value; OnPropertyChanged(nameof(SelectedExhibitsLocalizationSearchType)); }
        }

        private List<ExhibitLocalizationSortType> _exhibitsLocalizationSortTypes;
        public List<ExhibitLocalizationSortType> ExhibitsLocalizationSortTypes
        {
            get { return _exhibitsLocalizationSortTypes; }
            set { _exhibitsLocalizationSortTypes = value; OnPropertyChanged(nameof(ExhibitsLocalizationSortTypes)); }
        }

        private ExhibitLocalizationSortType _selectedExhibitsLocalizationSortType;
        public ExhibitLocalizationSortType SelectedExhibitsLocalizationSortType
        {
            get { return _selectedExhibitsLocalizationSortType; }
            set
            {
                _selectedExhibitsLocalizationSortType = value;
                OnPropertyChanged(nameof(SelectedExhibitsLocalizationSortType));
                SortLocalizationsBy(SelectedExhibitsLocalizationSortType);
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
