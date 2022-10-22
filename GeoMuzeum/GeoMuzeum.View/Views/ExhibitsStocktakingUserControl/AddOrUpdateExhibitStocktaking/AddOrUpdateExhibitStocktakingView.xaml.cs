using GeoMuzeum.Model;
using GeoMuzeum.View.ViewServices;
using System;
using System.Windows;

namespace GeoMuzeum.View.Views.ExhibitsStocktakingUserControl.AddOrUpdateExhibitStocktaking
{
    /// <summary>
    /// Interaction logic for AddOrUpdateExhibitStocktakingView.xaml
    /// </summary>
    public partial class AddOrUpdateExhibitStocktakingView : Window
    {
        private readonly User _user;

        public AddOrUpdateExhibitStocktakingView(User user)
        {
            _user = user;

            InitializeComponent();
            Loaded += AddOrUpdateExhibitView_Loaded;
        }

        private async void AddOrUpdateExhibitView_Loaded(object sender, RoutedEventArgs e)
        {
            var dataContext = ViewModelLocator.AddOrUpdateExhibitStocktakingViewModel;
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
