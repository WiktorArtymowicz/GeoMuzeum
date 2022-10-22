using GeoMuzeum.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public interface IExhibitLocalizationDataService
    {
        Task<List<ExhibitLocalization>> GetExhibitLocalizations();
        Task<List<ExhibitLocalization>> GetExhibitLocalizationsByName(string name);
        Task<List<ExhibitLocalization>> GetExhibitLocalizationsByInfo(string info);
        Task<ExhibitLocalization> GetExhibitLocalizationById(ExhibitLocalization exhibitLocalization);
        Task SaveNewExhibitLocalization(ExhibitLocalization exhibitLocalization);
        Task UdateExhibitLocalization(ExhibitLocalization exhibitLocalization);
        Task DeleteExhibitLocalization(ExhibitLocalization exhibitLocalization);
    }
}