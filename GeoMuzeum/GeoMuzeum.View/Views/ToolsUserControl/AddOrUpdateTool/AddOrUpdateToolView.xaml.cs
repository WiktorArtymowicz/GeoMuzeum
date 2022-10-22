using GeoMuzeum.Model;
using GeoMuzeum.View.ViewServices;
using System;
using System.Windows;

namespace GeoMuzeum.View.Views.ToolsUserControl.AddOrUpdateTool
{
    /// <summary>
    /// Interaction logic for AddOrUpdateToolView.xaml
    /// </summary>
    public partial class AddOrUpdateToolView : Window
    {
        private readonly User _user;

        public AddOrUpdateToolView(User user)
        {
            _user = user;

            InitializeComponent();
            Loaded += AddOrUpdateExhibitView_Loaded;
        }

        private async void AddOrUpdateExhibitView_Loaded(object sender, RoutedEventArgs e)
        {
            var dataContext = ViewModelLocator.AddOrUpdateToolViewModel;
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
