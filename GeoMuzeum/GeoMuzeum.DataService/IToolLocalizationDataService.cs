using GeoMuzeum.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public interface IToolLocalizationDataService
    {
        Task<List<ToolLocalization>> GetAllToolLocalizations();
        Task<List<ToolLocalization>> GetToolLocalizationsByNumber(string number);
        Task<List<ToolLocalization>> GetToolLocalizationsByInfo(string info);
        Task<ToolLocalization> GetToolLocalizationById(ToolLocalization toolLocalization);
        Task AddNewLocalization(ToolLocalization toolLocalization);
        Task UpdateLocalization(ToolLocalization toolLocalization);
        Task DeleteLocalization(ToolLocalization toolLocalization);
    }
}