using GeoMuzeum.View.ViewServices;
using System;
using System.Windows;

namespace GeoMuzeum.View.Views.LoginView
{
    /// <summary>
    /// Interaction logic for UserLoginView.xaml
    /// </summary>
    public partial class UserLoginView : Window
    {
        public UserLoginView()
        {
            InitializeComponent();
            Loaded += AddOrUpdateToolsLocalizationView_Loaded;
        }

        private void AddOrUpdateToolsLocalizationView_Loaded(object sender, RoutedEventArgs e)
        {
            var dataContext = ViewModelLocator.UserLoginViewModel;
            
            if(dataContext != null)
            {
                DataContext = dataContext;
                dataContext.CleanData();
                dataContext.CloseAction = new Action(Close);
            }
        }
    }
}
