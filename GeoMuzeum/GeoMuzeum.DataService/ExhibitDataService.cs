using GeoMuzeum.DataModel;
using GeoMuzeum.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public class ExhibitDataService : IExhibitDataService
    {
        public async Task<List<Exhibit>> GetAllExhibitsAsync()
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.Exhibits.AsNoTracking().Include(x => x.Localization).Include(x => x.Catalog).ToListAsync();
            }
        }

        public async Task<List<Exhibit>> GetAllExhibitsByNameAsync(string name)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                if (!await dbContext.Exhibits.AnyAsync())
                    return new List<Exhibit>();

                return await dbContext.Exhibits.AsNoTracking().Where(x => x.ExhibitName.ToLower().Contains(name.ToLower())).Include(x => x.Localization).Include(x => x.Catalog).ToListAsync();
            }
        }

        public async Task<List<Exhibit>> GetAllExhibitsByInfoAsync(string info)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                if (!await dbContext.Exhibits.AnyAsync())
                    return new List<Exhibit>();

                return await dbContext.Exhibits.AsNoTracking().Where(x => x.ExhibitInfo.ToLower().Contains(info.ToLower())).Include(x => x.Localization).Include(x => x.Catalog).ToListAsync();
            }
        }

        public async Task<List<Exhibit>> GetAllExhibitsByTypeAsync(string type)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                if (!await dbContext.Exhibits.AnyAsync())
                    return new List<Exhibit>();

                return await dbContext.Exhibits.AsNoTracking().Where(x => x.ExhibitType.ToString().ToLower().Contains(type.ToLower())).Include(x => x.Localization).Include(x => x.Catalog).ToListAsync();
            }
        }

        public async Task<List<Exhibit>> GetAllExhibitsByLocalizationAsync(string localization)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                if (!await dbContext.Exhibits.AnyAsync())
                    return new List<Exhibit>();

                return await dbContext.Exhibits.AsNoTracking().Where(x => x.Localization.ExhibitLocalizationNumber.ToLower().Contains(localization.ToLower())).Include(x => x.Localization).Include(x => x.Catalog).ToListAsync();
            }
        }

        public async Task<Exhibit> GetExhibitById(Exhibit exhibit)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.Exhibits.AsNoTracking().Include(x => x.Localization).Include(x => x.Catalog).FirstOrDefaultAsync(x => x.ExhibitId == exhibit.ExhibitId);
            }
        }

        public async Task SaveNewExhibit(Exhibit exhibit)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                try
                {
                    var foundCatalog = dbContext.Catalogs.AsNoTracking().Include(x => x.User).SingleOrDefault(x => x.CatalogId == exhibit.Catalog.CatalogId);
                    var foundExhibitLocalization = dbContext.ExhibitLocalizations.AsNoTracking().SingleOrDefault(x => x.ExhibitLocalizationId == exhibit.Localization.ExhibitLocalizationId);

                    dbContext.Entry(foundCatalog).State = EntityState.Unchanged;
                    dbContext.Entry(foundExhibitLocalization).State = EntityState.Unchanged;

                    dbContext.Catalogs.Attach(foundCatalog);
                    dbContext.ExhibitLocalizations.Attach(foundExhibitLocalization);

                    exhibit.Localization = foundExhibitLocalization;
                    exhibit.Catalog = foundCatalog;
                  
                    dbContext.Exhibits.Add(exhibit);
                    dbContext.Entry(exhibit).State = EntityState.Added;
                    await dbContext.SaveChangesAsync();
                }
                catch (System.Exception exception)
                {
                    throw exception;
                }
            }
        }

        public async Task UpdateExhibit(Exhibit exhibit)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                try
                {
                    var foundExhibit = await dbContext.Exhibits.FindAsync(exhibit.ExhibitId);
                    var foundCatalog = dbContext.Catalogs.AsNoTracking().Include(x => x.User).SingleOrDefault(x => x.CatalogId == exhibit.Catalog.CatalogId);
                    var foundExhibitLocalization = dbContext.ExhibitLocalizations.AsNoTracking().SingleOrDefault(x => x.ExhibitLocalizationId == exhibit.Localization.ExhibitLocalizationId);

                    dbContext.Entry(foundCatalog).State = EntityState.Unchanged;
                    dbContext.Entry(foundExhibitLocalization).State = EntityState.Unchanged;
                    
                    dbContext.Exhibits.Attach(foundExhibit);
                    dbContext.Catalogs.Attach(foundCatalog);
                    dbContext.ExhibitLocalizations.Attach(foundExhibitLocalization);

                    foundExhibit.ExhibitName = exhibit.ExhibitName;
                    foundExhibit.ExhibitDescription = exhibit.ExhibitDescription;
                    foundExhibit.ExhibitType = exhibit.ExhibitType;
                    foundExhibit.Localization = foundExhibitLocalization;
                    foundExhibit.Catalog = foundCatalog;

                    dbContext.SaveChanges();
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                
            }
        }

        public async Task DeleteExhibit(Exhibit exhibit)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                var foundExhibit = await dbContext.Exhibits.FindAsync(exhibit.ExhibitId);

                dbContext.Entry(foundExhibit).State = EntityState.Deleted;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAllTable()
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                await dbContext.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Exhibits");
            }
        }
    }
}
