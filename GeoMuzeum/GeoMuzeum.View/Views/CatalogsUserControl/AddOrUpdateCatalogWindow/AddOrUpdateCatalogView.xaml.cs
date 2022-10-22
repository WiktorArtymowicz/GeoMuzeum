using GeoMuzeum.Model;
using GeoMuzeum.View.ViewServices;
using System;
using System.Windows;

namespace GeoMuzeum.View.Views.CatalogsUserControl.AddOrUpdateCatalogWindow
{
    /// <summary>
    /// Interaction logic for AddOrUpdateCatalogView.xaml
    /// </summary>
    public partial class AddOrUpdateCatalogView : Window
    {
        private readonly User _user;

        public AddOrUpdateCatalogView(User user)
        {
            _user = user;

            InitializeComponent();
            Loaded += AddOrUpdateCatalogView_Loaded;
        }

        private async void AddOrUpdateCatalogView_Loaded(object sender, RoutedEventArgs e)
        {
            var dataContext = ViewModelLocator.AddOrUpdateCatalogViewModel;
            if (dataContext != null)
            { 
                DataContext = dataContext;
                await dataContext.LoadDataAsync();
                dataContext.SetUser(_user);
                dataContext.CloseAction = new Action(Close);
            }
        }
    }
}
