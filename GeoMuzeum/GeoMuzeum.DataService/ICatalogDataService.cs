using GeoMuzeum.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public interface ICatalogDataService
    {
        Task<List<Catalog>> GetAllCatalogs();
        Task<List<Catalog>> GetCatalogsByName(string name);
        Task<List<Catalog>> GetCatalogsByInfo(string info);
        Task<List<Catalog>> GetCatalogsByUser(string user);
        Task<bool> AnyCatalogsByUser(User user);
        Task<Catalog> GetCatalogsById(Catalog catalog);
        Task SaveNewCatalog(Catalog catalog);
        Task UpdateCatalog(Catalog catalog);
        Task DeleteCatalog(Catalog catalog);
    }
}