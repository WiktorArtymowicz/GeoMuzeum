using GeoMuzeum.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public interface IToolStocktakingDataService
    {
        Task<List<ToolStocktaking>> GetAllToolsStocktakings();
        Task<List<ToolStocktaking>> GetAllToolsStocktakingsByTool(string toolName);
        Task<List<ToolStocktaking>> GetAllToolsStocktakingsByLocalization(string localization);
        Task<ToolStocktaking> FindToolStocktakingPositionById(ToolStocktaking toolStocktaking);
        Task AddToolStocktakingPosition(ToolStocktaking toolStocktaking);
        Task UpdateToolStocktakingPosition(ToolStocktaking toolStocktaking);
        Task DeleteToolStocktakingPosition(ToolStocktaking toolStocktaking);
        Task DeleteAllTable();
        Task ConfirmStocktaking();
    }
}