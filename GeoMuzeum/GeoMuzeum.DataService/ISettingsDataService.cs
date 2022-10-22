using GeoMuzeum.Model;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public interface ISettingsDataService
    {
        Task CreateSettings(Settings settings);
        Task<Settings> GetSettings();
        Task UpdateSettings(Settings settings);
    }
}
