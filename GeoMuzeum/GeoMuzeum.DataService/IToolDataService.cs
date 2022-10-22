using GeoMuzeum.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public interface IToolDataService
    {
        Task<List<Tool>> GetAllTools();
        Task<List<Tool>> GetToolsByName(string name);
        Task<List<Tool>> GetToolsByInfo(string info);
        Task<List<Tool>> GetToolsByLocalization(string localization);
        Task<Tool> GetToolById(Tool tool);
        Task AddNewTool(Tool tool);
        Task UpdateTool(Tool tool);
        Task DeleteTool(Tool tool);
    }
}