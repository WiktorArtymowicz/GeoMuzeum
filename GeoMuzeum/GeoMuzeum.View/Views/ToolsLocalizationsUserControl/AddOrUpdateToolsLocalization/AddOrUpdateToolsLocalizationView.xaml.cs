using GeoMuzeum.Model;
using GeoMuzeum.View.ViewServices;
using System;
using System.Windows;

namespace GeoMuzeum.View.Views.ToolsLocalizationsUserControl.AddOrUpdateToolsLocalization
{
    /// <summary>
    /// Interaction logic for AddOrUpdateToolsLocalizationView.xaml
    /// </summary>
    public partial class AddOrUpdateToolsLocalizationView : Window
    {
        private readonly User _user;

        public AddOrUpdateToolsLocalizationView(User user)
        {
            _user = user;

            InitializeComponent();
            Loaded += AddOrUpdateToolsLocalizationView_Loaded;
        }

        private async void AddOrUpdateToolsLocalizationView_Loaded(object sender, RoutedEventArgs e)
        {
            var dataContext = ViewModelLocator.AddOrUpdateToolsLocalizationViewModel;

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
