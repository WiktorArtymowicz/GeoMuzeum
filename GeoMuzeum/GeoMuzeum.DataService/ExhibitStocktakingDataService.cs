using GeoMuzeum.DataModel;
using GeoMuzeum.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public class ExhibitStocktakingDataService : IExhibitStocktakingDataService
    {
        public async Task<List<ExhibitStocktaking>> GetAllExhibitStocktakingPositions()
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.ExhibitStocktakings.AsNoTracking().Include(x => x.Catalog).Include(x => x.Exhibit).Include(x => x.Localization).ToListAsync();
            }
        }

        public async Task<ExhibitStocktaking> FindExhibitStocktakingPositionById(ExhibitStocktaking exhibitStocktaking)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.ExhibitStocktakings.AsNoTracking().Include(x => x.Catalog).Include(x => x.Exhibit).Include(x => x.Localization).FirstOrDefaultAsync(x => x.ExhibitStocktakingId == exhibitStocktaking.ExhibitStocktakingId);
            }
        }

        public async Task AddExhibitStocktakingPosition(ExhibitStocktaking exhibitStocktaking)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                try
                {
                    var foundExhibit = await dbContext.Exhibits.AsNoTracking().SingleOrDefaultAsync(x => x.ExhibitId == exhibitStocktaking.Exhibit.ExhibitId);
                    var foundExhibitLocaliztion = await dbContext.ExhibitLocalizations.AsNoTracking().SingleOrDefaultAsync(x => x.ExhibitLocalizationId == exhibitStocktaking.Localization.ExhibitLocalizationId);
                    var foundCatalog = await dbContext.Catalogs.AsNoTracking().SingleOrDefaultAsync(x => x.CatalogId == exhibitStocktaking.Catalog.CatalogId);

                    dbContext.Entry(foundExhibit).State = EntityState.Unchanged;
                    dbContext.Entry(foundExhibitLocaliztion).State = EntityState.Unchanged;
                    dbContext.Entry(foundCatalog).State = EntityState.Unchanged;

                    dbContext.Exhibits.Attach(foundExhibit);
                    dbContext.ExhibitLocalizations.Attach(foundExhibitLocaliztion);
                    dbContext.Catalogs.Attach(foundCatalog);

                    exhibitStocktaking.Catalog = foundCatalog;
                    exhibitStocktaking.Localization = foundExhibitLocaliztion;
                    exhibitStocktaking.Exhibit = foundExhibit;

                    dbContext.ExhibitStocktakings.Add(exhibitStocktaking);
                    dbContext.Entry(exhibitStocktaking).State = EntityState.Added;
                    await dbContext.SaveChangesAsync();
                }
                catch (System.Exception exception)
                {
                    throw exception;
                }
            }
        }

        public async Task UpdateExhibitStocktakingPosition(ExhibitStocktaking exhibitStocktaking)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                try
                {
                    var foundExhibitStocktaking = await dbContext.ExhibitStocktakings.FirstOrDefaultAsync(x => x.ExhibitStocktakingId == exhibitStocktaking.ExhibitStocktakingId);
                    var foundExhibit = await dbContext.Exhibits.AsNoTracking().SingleOrDefaultAsync(x => x.ExhibitId == exhibitStocktaking.Exhibit.ExhibitId);
                    var foundExhibitLocaliztion = await dbContext.ExhibitLocalizations.AsNoTracking().SingleOrDefaultAsync(x => x.ExhibitLocalizationId == exhibitStocktaking.Localization.ExhibitLocalizationId);
                    var foundCatalog = await dbContext.Catalogs.AsNoTracking().SingleOrDefaultAsync(x => x.CatalogId == exhibitStocktaking.Catalog.CatalogId);

                    dbContext.Entry(foundExhibit).State = EntityState.Unchanged;
                    dbContext.Entry(foundExhibitLocaliztion).State = EntityState.Unchanged;
                    dbContext.Entry(foundCatalog).State = EntityState.Unchanged;

                    dbContext.ExhibitStocktakings.Attach(foundExhibitStocktaking);
                    dbContext.Exhibits.Attach(foundExhibit);
                    dbContext.ExhibitLocalizations.Attach(foundExhibitLocaliztion);
                    dbContext.Catalogs.Attach(foundCatalog);

                    foundExhibitStocktaking.Catalog = foundCatalog;
                    foundExhibitStocktaking.Localization = foundExhibitLocaliztion;
                    foundExhibitStocktaking.Exhibit = foundExhibit;

                    dbContext.Entry(foundExhibitStocktaking).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                }
                catch (System.Exception exception)
                {
                    throw exception;
                }
            }
        }

        public async Task DeleteExhibitStocktakingPosition(ExhibitStocktaking exhibitStocktaking)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                var foundExhibitStocktaking = await dbContext.ExhibitStocktakings.FindAsync(exhibitStocktaking.ExhibitStocktakingId);

                dbContext.Entry(foundExhibitStocktaking).State = EntityState.Deleted;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAllTable()
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                await dbContext.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE ExhibitStocktakings");
            }
        }

        public async Task ConfirmStocktaking()
        {
            var exhibitStocktakings = new List<ExhibitStocktaking>();

            using (var dbContext = new GeoMuzeumContext())
            {
                exhibitStocktakings = await dbContext.ExhibitStocktakings.AsNoTracking().Include(x => x.Catalog).Include(x => x.Exhibit).Include(x => x.Localization).ToListAsync();

                await dbContext.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE ExhibitStocktakings");
            }

            try
            {
                foreach (var exhibitStocktaking in exhibitStocktakings)
                {
                    using (var dbContext = new GeoMuzeumContext())
                    {
                        var foundExhibit = await dbContext.Exhibits.FindAsync(exhibitStocktaking.Exhibit.ExhibitId);
                        var foundCatalog = dbContext.Catalogs.AsNoTracking().Include(x => x.User).SingleOrDefault(x => x.CatalogId == exhibitStocktaking.Catalog.CatalogId);
                        var foundExhibitLocalization = dbContext.ExhibitLocalizations.AsNoTracking().SingleOrDefault(x => x.ExhibitLocalizationId == exhibitStocktaking.Localization.ExhibitLocalizationId);

                        dbContext.Entry(foundExhibitLocalization).State = EntityState.Unchanged;
                        dbContext.Entry(foundCatalog).State = EntityState.Unchanged;

                        dbContext.ExhibitLocalizations.Attach(foundExhibitLocalization);
                        dbContext.Catalogs.Attach(foundCatalog);
                        dbContext.Exhibits.Attach(foundExhibit);

                        foundExhibit.Catalog = null;
                        foundExhibit.Localization = null;

                        foundExhibit.Catalog = foundCatalog;
                        foundExhibit.Localization = foundExhibitLocalization;

                        await dbContext.SaveChangesAsync();
                    }
                }
            }
            catch (System.Exception exception)
            {
                throw exception;
            }
        }

        public async Task<List<ExhibitStocktaking>> GetAllExhibitStocktakingPositionsByCatalog(string catalogName)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                if (!await dbContext.ExhibitStocktakings.AnyAsync())
                    return new List<ExhibitStocktaking>();

                return await dbContext.ExhibitStocktakings.AsNoTracking().Include(x => x.Catalog).Include(x => x.Exhibit).Include(x => x.Localization).Where(x => x.Catalog.CatalogName.ToLower().Contains(catalogName.ToLower())).ToListAsync();
            }
        }

        public async Task<List<ExhibitStocktaking>> GetAllExhibitStocktakingPositionsByExhibit(string exhibitName)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                if (!await dbContext.ExhibitStocktakings.AnyAsync())
                    return new List<ExhibitStocktaking>();

                return await dbContext.ExhibitStocktakings.AsNoTracking().Include(x => x.Catalog).Include(x => x.Exhibit).Include(x => x.Localization).Where(x => x.Exhibit.ExhibitName.ToLower().Contains(exhibitName.ToLower())).ToListAsync();
            }
        }

        public async Task<List<ExhibitStocktaking>> GetAllExhibitStocktakingPositionsByLocalization(string localizationName)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                if (!await dbContext.ExhibitStocktakings.AnyAsync())
                    return new List<ExhibitStocktaking>();

                return await dbContext.ExhibitStocktakings.AsNoTracking().Include(x => x.Catalog).Include(x => x.Exhibit).Include(x => x.Localization).Where(x => x.Localization.ExhibitLocalizationNumber.ToLower().Contains(localizationName.ToLower())).ToListAsync();
            }
        }
    }
}
