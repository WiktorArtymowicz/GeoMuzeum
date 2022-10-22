using GeoMuzeum.Model;
using System.ComponentModel;
using System.Threading.Tasks;

namespace GeoMuzeum.View.ViewServices
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual async Task LoadDataAsync()
        {

        }

        public virtual void SetUser(User user)
        {

        }
    }
}
