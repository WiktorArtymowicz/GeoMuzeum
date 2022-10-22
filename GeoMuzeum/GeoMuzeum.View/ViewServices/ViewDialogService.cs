using System.Windows;

namespace GeoMuzeum.View.ViewServices
{
    public class ViewDialogService<T> : IViewDialogService where T : Window
    {
        Window genericWindow = null;

        public ViewDialogService(T window)
        {
            genericWindow = window;
        }

        public void ShowGenericWindow()
        {
            genericWindow.ShowDialog();
        }

        public void CloseGenericWindow()
        {
            if (genericWindow != null)
                genericWindow.Close();
        }
    }
}
