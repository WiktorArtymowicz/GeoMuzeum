using GeoMuzeum.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public interface IExhibitStocktakingDataService
    {
        Task<List<ExhibitStocktaking>> GetAllExhibitStocktakingPositions();
        Task<List<ExhibitStocktaking>> GetAllExhibitStocktakingPositionsByCatalog(string catalogName);
        Task<List<ExhibitStocktaking>> GetAllExhibitStocktakingPositionsByExhibit(string exhibitName);
        Task<List<ExhibitStocktaking>> GetAllExhibitStocktakingPositionsByLocalization(string localizationName);
        Task<ExhibitStocktaking> FindExhibitStocktakingPositionById(ExhibitStocktaking exhibitStocktaking);
        Task AddExhibitStocktakingPosition(ExhibitStocktaking exhibitStocktaking);
        Task UpdateExhibitStocktakingPosition(ExhibitStocktaking exhibitStocktaking);
        Task DeleteExhibitStocktakingPosition(ExhibitStocktaking exhibitStocktaking);
        Task DeleteAllTable();
        Task ConfirmStocktaking();
    }
}
