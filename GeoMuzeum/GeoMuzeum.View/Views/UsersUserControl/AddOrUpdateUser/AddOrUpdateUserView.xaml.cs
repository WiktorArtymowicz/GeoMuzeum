using GeoMuzeum.Model;
using GeoMuzeum.View.ViewServices;
using System;
using System.Windows;

namespace GeoMuzeum.View.Views.UsersUserControl.AddOrUpdateUser
{
    /// <summary>
    /// Interaction logic for AddOrUpdateUserView.xaml
    /// </summary>
    public partial class AddOrUpdateUserView : Window
    {
        private readonly User _user;

        public AddOrUpdateUserView(User user)
        {
            _user = user;

            InitializeComponent();
            Loaded += AddOrUpdateUserView_Loaded;
        }

        private async void AddOrUpdateUserView_Loaded(object sender, RoutedEventArgs e)
        {
            var dataContext = ViewModelLocator.AddOrUpdateUserViewModel;
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
