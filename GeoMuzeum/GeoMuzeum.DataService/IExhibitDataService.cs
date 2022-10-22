using GeoMuzeum.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public interface IExhibitDataService
    {
        Task<List<Exhibit>> GetAllExhibitsAsync();
        Task<List<Exhibit>> GetAllExhibitsByNameAsync(string name);
        Task<List<Exhibit>> GetAllExhibitsByInfoAsync(string info);
        Task<List<Exhibit>> GetAllExhibitsByTypeAsync(string type);
        Task<List<Exhibit>> GetAllExhibitsByLocalizationAsync(string localization);
        Task<Exhibit> GetExhibitById(Exhibit exhibit);
        Task SaveNewExhibit(Exhibit exhibit);
        Task UpdateExhibit(Exhibit exhibit);
        Task DeleteExhibit(Exhibit exhibit);
        Task DeleteAllTable();
    }
}