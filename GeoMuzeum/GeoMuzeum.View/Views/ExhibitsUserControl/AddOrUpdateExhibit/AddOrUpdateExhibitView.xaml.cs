using GeoMuzeum.Model;
using GeoMuzeum.View.ViewServices;
using System;
using System.Windows;

namespace GeoMuzeum.View.Views.ExhibitsUserControl.AddOrUpdateExhibit
{
    public partial class AddOrUpdateExhibitView : Window
    {
        private readonly User _user;

        public AddOrUpdateExhibitView(User user)
        {
            _user = user;

            InitializeComponent();
            Loaded += AddOrUpdateExhibitView_Loaded;
        }

        private async void AddOrUpdateExhibitView_Loaded(object sender, RoutedEventArgs e)
        {
            var dataContext = ViewModelLocator.AddOrUpdateExhibitViewModel;
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
