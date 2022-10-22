using GeoMuzeum.Model;
using GeoMuzeum.View.ViewServices;
using System;
using System.Windows;

namespace GeoMuzeum.View.Views.ToolsStocktakingUserControl.AddOrUpdateToolStocktaking
{
    /// <summary>
    /// Interaction logic for AddOrUpdateToolsStocktakingView.xaml
    /// </summary>
    public partial class AddOrUpdateToolsStocktakingView : Window
    {
        private readonly User _user;

        public AddOrUpdateToolsStocktakingView(User user)
        {
            _user = user;

            InitializeComponent();
            Loaded += AddOrUpdateExhibitView_Loaded;
        }

        private async void AddOrUpdateExhibitView_Loaded(object sender, RoutedEventArgs e)
        {
            var dataContext = ViewModelLocator.addOrUpdateToolsStocktakingViewModel;
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
