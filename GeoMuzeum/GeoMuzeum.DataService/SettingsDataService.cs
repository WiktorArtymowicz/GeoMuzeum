using GeoMuzeum.DataModel;
using GeoMuzeum.Model;
using System.Data.Entity;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public class SettingsDataService : ISettingsDataService
    {
        public async Task CreateSettings(Settings settings)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                dbContext.Settings.Add(settings);

                dbContext.Entry(settings).State = EntityState.Added;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<Settings> GetSettings()
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.Settings.AsNoTracking().FirstOrDefaultAsync();
            }
        }

        public async Task UpdateSettings(Settings settings)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                var foundSettings = await dbContext.Settings.FindAsync(settings.SettingsId);

                foundSettings.IsExhibitStocktaking = settings.IsExhibitStocktaking;
                foundSettings.IsToolStocktaking = settings.IsToolStocktaking;

                dbContext.Entry(settings).State = EntityState.Added;
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
