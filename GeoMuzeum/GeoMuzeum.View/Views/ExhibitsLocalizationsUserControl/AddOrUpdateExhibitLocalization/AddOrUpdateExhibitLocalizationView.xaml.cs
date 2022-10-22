using GeoMuzeum.Model;
using GeoMuzeum.View.ViewServices;
using System;
using System.Windows;

namespace GeoMuzeum.View.Views.ExhibitsLocalizationsUserControl.AddOrUpdateExhibitLocalization
{
    /// <summary>
    /// Interaction logic for AddOrUpdateExhibitLocalizationView.xaml
    /// </summary>
    public partial class AddOrUpdateExhibitLocalizationView : Window
    {
        private readonly User _user;

        public AddOrUpdateExhibitLocalizationView(User user)
        {
            _user = user;

            InitializeComponent();
            Loaded += AddOrUpdateExhibitLocalizationView_Loaded;
        }

        private async void AddOrUpdateExhibitLocalizationView_Loaded(object sender, RoutedEventArgs e)
        {
            var dataContext = ViewModelLocator.AddOrUpdateExhibitLocalizationViewModel;

            if(dataContext != null)
            {
                DataContext = dataContext;
                await dataContext.LoadDataAsync();
                dataContext.SetUser(_user);
                dataContext.CloseAction = new Action(Close);
            }
        }
    }
}
