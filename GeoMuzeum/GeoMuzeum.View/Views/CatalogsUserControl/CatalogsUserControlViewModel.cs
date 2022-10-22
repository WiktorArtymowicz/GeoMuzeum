using GeoMuzeum.DataService;
using GeoMuzeum.Model;
using GeoMuzeum.View.ProjectHelpers;
using GeoMuzeum.View.Views.CatalogsUserControl.AddOrUpdateCatalogWindow;
using GeoMuzeum.View.ViewServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GeoMuzeum.View.Views.CatalogsUserControl
{
    public class CatalogsUserControlViewModel : BaseViewModel
    {
        private ICatalogDataService _catalogDataService;
        private readonly IUserLogDataService _userLogDataService;

        private User _sentUser;

        protected CatalogsUserControlViewModel()
        {

        }

        public CatalogsUserControlViewModel(ICatalogDataService catalogDataService, IUserLogDataService userLogDataService)
        {
            _catalogDataService = catalogDataService;
            _userLogDataService = userLogDataService;

            Catalogs = new ObservableCollection<Catalog>();
            Exhibits = new ObservableCollection<Exhibit>();

            CatalogSearchTypes = new List<CatalogSearchType>();
            CatalogSortTypes = new List<CatalogSortType>();
        }

        public override async Task LoadDataAsync()
        {
            try
            {
                await LoadCatalogs();
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
            CatalogSortTypes.Clear();

            CatalogSortTypes = Enum.GetValues(typeof(CatalogSortType)).Cast<CatalogSortType>().ToList();
            SelectedCatalogSortType = CatalogSortTypes.FirstOrDefault();
        }

        private void LoadSearchTypes()
        {
            CatalogSearchTypes.Clear();

            CatalogSearchTypes = Enum.GetValues(typeof(CatalogSearchType)).Cast<CatalogSearchType>().ToList();
            SelectedCatalogSearchType = CatalogSearchTypes.FirstOrDefault();
        }

        private async Task LoadCatalogs()
        {
            Catalogs.Clear();

            var catalogsFormDb = await _catalogDataService.GetAllCatalogs();
            Catalogs = catalogsFormDb.ToObservableCollection();

            SelectedCatalog = Catalogs.FirstOrDefault();
        }

        private async void SearchCatalogsBy(string searchText)
        {
           if (string.IsNullOrWhiteSpace(searchText))
            {
                await LoadCatalogs();
                return;
            }

            if (SelectedCatalogSearchType == CatalogSearchType.Nazwa)
            {
                var catalogsFormDb = await _catalogDataService.GetCatalogsByName(searchText);
                Catalogs = catalogsFormDb.ToObservableCollection();
            }

            if (SelectedCatalogSearchType == CatalogSearchType.Opis)
            {
                var catalogsFormDb = await _catalogDataService.GetCatalogsByInfo(searchText);
                Catalogs = catalogsFormDb.ToObservableCollection();
            }

            if (SelectedCatalogSearchType == CatalogSearchType.Użytkownik)
            {
                var catalogsFormDb = await _catalogDataService.GetCatalogsByUser(searchText);
                Catalogs = catalogsFormDb.ToObservableCollection();
            }
        }

        private void SortCatalogsBy(CatalogSortType catalogSortType)
        {
            if (catalogSortType == CatalogSortType.Domyślnie)
                Catalogs = Catalogs.OrderBy(x => x.CatalogId).ToObservableCollection();

            if(catalogSortType == CatalogSortType.Nazwa)
                Catalogs = Catalogs.OrderByDescending(x => x.CatalogName).ToObservableCollection();

            if(catalogSortType == CatalogSortType.Opis)
                Catalogs = Catalogs.OrderByDescending(x => x.CatalogName).ToObservableCollection();

            if (catalogSortType == CatalogSortType.Użytkownik)
                Catalogs = Catalogs.OrderByDescending(x => x.User.UserName).ToObservableCollection();
        }

        private async void AddCatalog()
        {
            ViewMessenger.Default.Send<Catalog>(null);

            var catalogViewService = new ViewDialogService<AddOrUpdateCatalogView>(new AddOrUpdateCatalogView(_sentUser));
            catalogViewService.ShowGenericWindow();

            await LoadDataAsync();
        }

        private async void EditCatalog()
        {
            ViewMessenger.Default.Send(SelectedCatalog);

            var catalogViewService = new ViewDialogService<AddOrUpdateCatalogView>(new AddOrUpdateCatalogView(_sentUser));
            catalogViewService.ShowGenericWindow();

            await LoadDataAsync();
        }

        private async void DeleteCatalog()
        {
            if (SelectedCatalog == null)
                return;
            try
            {
                var question = MessageBox.Show($"Czy usunąć katalog {SelectedCatalog.CatalogName}?", "Pytanie", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (question == MessageBoxResult.No)
                    return;

                if (SelectedCatalog.Exhibits.Any())
                {
                    MessageBox.Show("Nie można usunąć katalogu ponieważ są do niego przypisane eksponaty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await _catalogDataService.DeleteCatalog(SelectedCatalog);
                await _userLogDataService.AddUserLog(new UserLog($"Użytkownik usunął katalog {SelectedCatalog.CatalogName}", _sentUser));
                await LoadDataAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private ICommand _addCatalogCommand;
        public ICommand AddCatalogCommand
        {
            get
            {
                if (_addCatalogCommand == null)
                    _addCatalogCommand = new CustomCommand((object o) => { AddCatalog(); }, (object o) => { return true; });

                return _addCatalogCommand;
            }
            set { _addCatalogCommand = value; }
        }

        private ICommand _editCatalogCommand;
        public ICommand EditCatalogCommand
        {
            get
            {
                if (_editCatalogCommand == null)
                    _editCatalogCommand = new CustomCommand((object o) => { EditCatalog(); }, (object o) => { return true; });

                return _editCatalogCommand;
            }
            set { _editCatalogCommand = value; }
        }

        private ICommand _deleteCatalogCommand;
        public ICommand DeleteCatalogCommand
        {
            get
            {
                if (_deleteCatalogCommand == null)
                    _deleteCatalogCommand = new CustomCommand((object o) => { DeleteCatalog(); }, (object o) => { return true; });

                return _deleteCatalogCommand;
            }
            set { _deleteCatalogCommand = value; }
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
            set
            {
                _selectedCatalog = value;
                OnPropertyChanged(nameof(SelectedCatalog));

                Exhibits = SelectedCatalog != null ? SelectedCatalog.Exhibits.ToObservableCollection() : new ObservableCollection<Exhibit>();
            }
        }

        private ObservableCollection<Exhibit> _exhibits;

        public ObservableCollection<Exhibit> Exhibits
        {
            get { return _exhibits; }
            set { _exhibits = value; OnPropertyChanged(nameof(Exhibits)); }
        }

        private string _catalogName;

        public string CatalogName
        {
            get { return _catalogName; }
            set { _catalogName = value; OnPropertyChanged(nameof(CatalogName)); }
        }

        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; OnPropertyChanged(nameof(UserName)); }
        }

        private string _catalogDescription;

        public string CatalogDescription
        {
            get { return _catalogDescription; }
            set { _catalogDescription = value; OnPropertyChanged(nameof(CatalogDescription)); }
        }

        private List<CatalogSearchType> _catalogSearchTypes;

        public List<CatalogSearchType> CatalogSearchTypes
        {
            get { return _catalogSearchTypes; }
            set { _catalogSearchTypes = value; OnPropertyChanged(nameof(CatalogSearchTypes)); }
        }

        private CatalogSearchType _selectedCatalogSearchType;

        public CatalogSearchType SelectedCatalogSearchType
        {
            get { return _selectedCatalogSearchType; }
            set { _selectedCatalogSearchType = value; OnPropertyChanged(nameof(SelectedCatalogSearchType)); }
        }

        private List<CatalogSortType> _catalogSortTypes;

        public List<CatalogSortType> CatalogSortTypes
        {
            get { return _catalogSortTypes; }
            set { _catalogSortTypes = value; OnPropertyChanged(nameof(CatalogSortTypes)); }
        }

        private CatalogSortType _selectedCatalogSortType;

        public CatalogSortType SelectedCatalogSortType
        {
            get { return _selectedCatalogSortType; }
            set
            {
                _selectedCatalogSortType = value;
                OnPropertyChanged(nameof(SelectedCatalogSortType));
                SortCatalogsBy(SelectedCatalogSortType);
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
                SearchCatalogsBy(SearchText);
            }
        }
    }
}
