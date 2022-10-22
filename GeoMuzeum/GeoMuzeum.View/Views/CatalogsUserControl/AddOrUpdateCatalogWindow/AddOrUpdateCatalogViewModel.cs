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

namespace GeoMuzeum.View.Views.CatalogsUserControl.AddOrUpdateCatalogWindow
{
    public class AddOrUpdateCatalogViewModel : BaseViewModel
    {
        private readonly IUserDataService _userDataService;
        private readonly ICatalogDataService _catalogDataService;
        private readonly IUserLogDataService _userLogDataService;

        private Catalog _sentCatalog;
        private User _sentUser;
        private EditStatusType _editStatusType;

        public AddOrUpdateCatalogViewModel()
        {

        }

        public AddOrUpdateCatalogViewModel(IUserDataService userDataService, ICatalogDataService catalogDataService, IUserLogDataService userLogDataService)
        {
            _userDataService = userDataService;
            _catalogDataService = catalogDataService;
            _userLogDataService = userLogDataService;

            ViewMessenger.Default.Register<Catalog>(this, OnCatalogRecived);
        }

        private void OnCatalogRecived(Catalog catalog)
        {
            _sentCatalog = catalog;
            _editStatusType = _sentCatalog == null ? EditStatusType.Add : EditStatusType.Modify;
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
            var catalog = _sentCatalog == null ? new Catalog() : await _catalogDataService.GetCatalogsById(_sentCatalog);
            Catalog = catalog;

            CatalogName = Catalog.CatalogName;
            CatalogDescription = Catalog.CatalogDescription;
        }

        public Action CloseAction { get; set; }

        private void CloseWindow()
        {
            CloseAction();
        }

        private async void AddOrUpdateCatalog(Catalog catalog)
        {
            try
            {
                var validateResult = ValidateCatalog(catalog);

                if (validateResult == false)
                    return;

                if (_editStatusType == EditStatusType.Add)
                {
                    await _catalogDataService.SaveNewCatalog(catalog);
                    await _userLogDataService.AddUserLog(new UserLog($"Użytkownik dodał katalog {catalog.CatalogName}", _sentUser));
                }
                   
                else
                {
                    await _catalogDataService.UpdateCatalog(catalog);
                    await _userLogDataService.AddUserLog(new UserLog($"Użytkownik edytował katalog {catalog.CatalogName}", _sentUser));
                }

                CloseWindow();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private bool ValidateCatalog(Catalog catalog)
        {
            if (string.IsNullOrWhiteSpace(catalog.CatalogName))
            {
                MessageBox.Show($"Proszę uzupełnić nazwę katalogu.", "Brak wprowadzonych danych.", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (catalog.User == null)
            {
                MessageBox.Show($"Proszę uzupełnić użytkownika.", "Brak wprowadzonych danych.", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private Catalog FillCatalogToSave()
        {
            Catalog.CatalogDescription = CatalogDescription;
            Catalog.CatalogName = CatalogName;
            Catalog.User = _sentUser;
            Catalog.Exhibits = Catalog.Exhibits;

            return Catalog;
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
                    _saveCommand = new CustomCommand((object o) => { AddOrUpdateCatalog(FillCatalogToSave()); }, (object o) => { return true; });

                return _saveCommand;
            }
            set { _saveCommand = value; }
        }

        private Catalog _catalog;

        public Catalog Catalog
        {
            get { return _catalog; }
            set { _catalog = value; OnPropertyChanged(nameof(Catalog)); }
        }

        private string _catalogName;

        public string CatalogName
        {
            get { return _catalogName; }
            set { _catalogName = value; OnPropertyChanged(nameof(CatalogName)); }
        }

        private string _catalogDescription;

        public string CatalogDescription
        {
            get { return _catalogDescription; }
            set { _catalogDescription = value; OnPropertyChanged(nameof(CatalogDescription)); }
        }
    }
}
