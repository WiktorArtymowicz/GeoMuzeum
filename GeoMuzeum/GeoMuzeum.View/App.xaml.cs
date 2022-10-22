using GeoMuzeum.View.Views.MainWindow;
using GeoMuzeum.View.Views.LoginView;
using GeoMuzeum.View.ViewServices;
using System.Windows;

namespace GeoMuzeum.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var loginView = new UserLoginView();
            loginView.Show();
        }
    }
}
