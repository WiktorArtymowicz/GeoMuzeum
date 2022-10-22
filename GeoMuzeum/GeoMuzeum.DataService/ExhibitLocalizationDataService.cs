using GeoMuzeum.DataModel;
using GeoMuzeum.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public class ExhibitLocalizationDataService : IExhibitLocalizationDataService
    {
        public async Task<List<ExhibitLocalization>> GetExhibitLocalizations()
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.ExhibitLocalizations.AsNoTracking().Include(x => x.Exhibits).ToListAsync();
            }
        }

        public async Task<List<ExhibitLocalization>> GetExhibitLocalizationsByName(string name)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                if (!await dbContext.ExhibitLocalizations.AnyAsync())
                    return new List<ExhibitLocalization>();

                return await dbContext.ExhibitLocalizations.AsNoTracking().Where(x => x.ExhibitLocalizationNumber.ToLower().Contains(name.ToLower())).Include(x => x.Exhibits).ToListAsync();
            }
        }

        public async Task<List<ExhibitLocalization>> GetExhibitLocalizationsByInfo(string info)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                if (!await dbContext.ExhibitLocalizations.AnyAsync())
                    return new List<ExhibitLocalization>();

                return await dbContext.ExhibitLocalizations.AsNoTracking().Where(x => x.ExhibitLocalizationDescription.ToLower().Contains(info.ToLower())).Include(x => x.Exhibits).ToListAsync();
            }
        }

        public async Task<ExhibitLocalization> GetExhibitLocalizationById(ExhibitLocalization exhibitLocalization)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.ExhibitLocalizations.AsNoTracking().Include(x => x.Exhibits).FirstOrDefaultAsync(x => x.ExhibitLocalizationId == exhibitLocalization.ExhibitLocalizationId);
            }
        }

        public async Task SaveNewExhibitLocalization(ExhibitLocalization exhibitLocalization)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                dbContext.ExhibitLocalizations.Add(exhibitLocalization);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UdateExhibitLocalization(ExhibitLocalization exhibitLocalization)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                try
                {
                    var foundExhibitLocalization = await dbContext.ExhibitLocalizations.FindAsync(exhibitLocalization.ExhibitLocalizationId);

                    foundExhibitLocalization.ExhibitLocalizationNumber = exhibitLocalization.ExhibitLocalizationNumber;
                    foundExhibitLocalization.ExhibitLocalizationDescription = exhibitLocalization.ExhibitLocalizationDescription;

                    dbContext.Entry(foundExhibitLocalization).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                }
                catch (System.Exception exception)
                {
                    throw exception;
                }
            }
        }

        public async Task DeleteExhibitLocalization(ExhibitLocalization exhibitLocalization)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                var foundExhibitLocalization = await dbContext.ExhibitLocalizations.FindAsync(exhibitLocalization.ExhibitLocalizationId);

                dbContext.Entry(foundExhibitLocalization).State = EntityState.Deleted;
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
