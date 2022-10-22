using GeoMuzeum.DataModel;
using GeoMuzeum.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public class CatalogDataService : ICatalogDataService
    {
        public async Task<List<Catalog>> GetAllCatalogs()
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.Catalogs.AsNoTracking().Include(x => x.User).Include(x => x.Exhibits).ToListAsync();
            }
        }

        public async Task<List<Catalog>> GetCatalogsByName(string name)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.Catalogs.AsNoTracking().Where(x => x.CatalogName.ToLower().Contains(name.ToLower())).Include(x => x.User).Include(x => x.Exhibits).ToListAsync();
            }
        }

        public async Task<List<Catalog>> GetCatalogsByInfo(string info)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                if (!await dbContext.Catalogs.AnyAsync())
                    return new List<Catalog>();

                return await dbContext.Catalogs.AsNoTracking().Where(x => x.CatalogDescription.ToLower().Contains(info.ToLower())).Include(x => x.User).Include(x => x.Exhibits).ToListAsync();
            }
        }

        public async Task<List<Catalog>> GetCatalogsByUser(string user)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                if (!await dbContext.Catalogs.AnyAsync())
                    return new List<Catalog>();

                return await dbContext.Catalogs.AsNoTracking().Where(x => x.User.UserName.ToLower().Contains(user.ToLower())).Include(x => x.User).Include(x => x.Exhibits).ToListAsync();
            }
        }

        public async Task<Catalog> GetCatalogsById(Catalog catalog)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.Catalogs.AsNoTracking().Include(x => x.User).Include(x => x.Exhibits).FirstOrDefaultAsync(x => x.CatalogId == catalog.CatalogId);
            }
        }

        public async Task SaveNewCatalog(Catalog catalog)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                dbContext.Users.Attach(catalog.User);

                dbContext.Catalogs.Add(catalog);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateCatalog(Catalog catalog)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                try
                {
                    var foundCatalog = await dbContext.Catalogs.FindAsync(catalog.CatalogId);

                    foundCatalog.CatalogDescription = catalog.CatalogDescription;
                    foundCatalog.CatalogName = catalog.CatalogName;
                    foundCatalog.User = catalog.User;

                    dbContext.Users.Attach(foundCatalog.User);

                    dbContext.Entry(foundCatalog).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                }
                catch (System.Exception exception)
                {
                    throw exception;
                }
            }
        }

        public async Task DeleteCatalog(Catalog catalog)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                var foundCatalog = await dbContext.Catalogs.FindAsync(catalog.CatalogId);

                dbContext.Entry(foundCatalog).State = EntityState.Deleted;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> AnyCatalogsByUser(User user)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.Catalogs.AsNoTracking().Include(x => x.User).Include(x => x.Exhibits).Where(x => x.User.UserId == user.UserId).AnyAsync();
            }
        }
    }
}
