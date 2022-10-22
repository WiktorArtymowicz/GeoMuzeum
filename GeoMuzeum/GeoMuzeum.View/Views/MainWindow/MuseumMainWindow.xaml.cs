using System;
using System.Windows;

namespace GeoMuzeum.View.Views.MainWindow
{
    public partial class MuseumMainWindow : Window
    {
        private readonly MuseumMainWindowViewModel _museumMainWindowViewModel;

        public MuseumMainWindow(MuseumMainWindowViewModel museumMainWindowViewModel)
        {
            InitializeComponent();
            _museumMainWindowViewModel = museumMainWindowViewModel;
            DataContext = _museumMainWindowViewModel;
            Loaded += MuseumMainWindow_Loaded;
        }

        private async void MuseumMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _museumMainWindowViewModel.LoadDataAsync();
            _museumMainWindowViewModel.CloseAction = new Action(Close);
        }
    }
}
