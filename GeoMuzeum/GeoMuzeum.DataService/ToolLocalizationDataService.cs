using GeoMuzeum.DataModel;
using GeoMuzeum.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public class ToolLocalizationDataService : IToolLocalizationDataService
    {
        public async Task<List<ToolLocalization>> GetAllToolLocalizations()
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.ToolLocalizations.AsNoTracking().Include(x => x.Tools).ToListAsync();
            }
        }

        public async Task<List<ToolLocalization>> GetToolLocalizationsByNumber(string number)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                if (!await dbContext.ToolStocktakings.AnyAsync())
                    return new List<ToolLocalization>();

                return await dbContext.ToolLocalizations.AsNoTracking().Where(x => x.ToolLocalizationNumber.ToLower().Contains(number.ToLower())).Include(x => x.Tools).ToListAsync();
            }
        }

        public async Task<List<ToolLocalization>> GetToolLocalizationsByInfo(string info)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                if (!await dbContext.ToolStocktakings.AnyAsync())
                    return new List<ToolLocalization>();

                return await dbContext.ToolLocalizations.AsNoTracking().Where(x => x.ToolLocalizationDescription.ToLower().Contains(info.ToLower())).Include(x => x.Tools).ToListAsync();
            }
        }

        public async Task<ToolLocalization> GetToolLocalizationById(ToolLocalization toolLocalization)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.ToolLocalizations.AsNoTracking().Include(x => x.Tools).FirstOrDefaultAsync(x => x.ToolLocalizationId == toolLocalization.ToolLocalizationId);
            }
        }

        public async Task AddNewLocalization(ToolLocalization toolLocalization)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                dbContext.ToolLocalizations.Add(toolLocalization);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateLocalization(ToolLocalization toolLocalization)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                try
                {
                    var foundLocalization = await dbContext.ToolLocalizations.FindAsync(toolLocalization.ToolLocalizationId);

                    foundLocalization.ToolLocalizationNumber = toolLocalization.ToolLocalizationNumber;
                    foundLocalization.ToolLocalizationDescription = toolLocalization.ToolLocalizationDescription;

                    dbContext.Entry(foundLocalization).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                }
                catch (System.Exception exception)
                {
                    throw exception;
                }
            }
        }

        public async Task DeleteLocalization(ToolLocalization toolLocalization)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                var foundLocalization = await dbContext.ToolLocalizations.FindAsync(toolLocalization.ToolLocalizationId);

                dbContext.Entry(foundLocalization).State = EntityState.Deleted;
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
