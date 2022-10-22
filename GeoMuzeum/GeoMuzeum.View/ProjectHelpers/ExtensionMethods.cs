using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GeoMuzeum.View.ProjectHelpers
{
    public static class ExtensionMethods
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            var observableCollection = new ObservableCollection<T>();

            foreach (var item in collection)
                observableCollection.Add(item);

            return observableCollection;
        }

        public static bool IsNotInt(this string s)
        {
            int x = 0;
            return !int.TryParse(s, out _);
        }

        public static int StringToInt(this string s)
        {
            var tryParse = int.TryParse(s, out int value);

            return tryParse ? value : 0;
        }
    }
}
