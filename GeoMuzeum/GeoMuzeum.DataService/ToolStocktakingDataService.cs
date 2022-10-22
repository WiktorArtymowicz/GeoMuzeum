using GeoMuzeum.DataModel;
using GeoMuzeum.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public class ToolStocktakingDataService : IToolStocktakingDataService
    {
        public async Task<List<ToolStocktaking>> GetAllToolsStocktakings()
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.ToolStocktakings.AsNoTracking().Include(x=> x.Localization).Include(x => x.Tool).Include(x => x.Tool).ToListAsync();
            }
        }

        public async Task<ToolStocktaking> FindToolStocktakingPositionById(ToolStocktaking toolStocktaking)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.ToolStocktakings.AsNoTracking().Include(x => x.Localization).Include(x => x.Tool).FirstOrDefaultAsync(x => x.ToolStocktakingId == toolStocktaking.ToolStocktakingId);
            };
        }

        public async Task AddToolStocktakingPosition(ToolStocktaking toolStocktaking)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                try
                {
                    var foundTool = await dbContext.Tools.AsNoTracking().SingleOrDefaultAsync(x => x.ToolId == toolStocktaking.Tool.ToolId);
                    var foundToolLocalization = await dbContext.ToolLocalizations.AsNoTracking().SingleOrDefaultAsync(x => x.ToolLocalizationId == toolStocktaking.Localization.ToolLocalizationId);

                    dbContext.Entry(foundTool).State = EntityState.Unchanged;
                    dbContext.Entry(foundToolLocalization).State = EntityState.Unchanged;

                    dbContext.Tools.Attach(foundTool);
                    dbContext.ToolLocalizations.Attach(foundToolLocalization);

                    toolStocktaking.Localization = foundToolLocalization;
                    toolStocktaking.Tool = foundTool;

                    dbContext.ToolStocktakings.Add(toolStocktaking);
                    dbContext.Entry(toolStocktaking).State = EntityState.Added;
                    await dbContext.SaveChangesAsync();
                }
                catch (System.Exception exception)
                {
                    throw exception;
                }
            }
        }

        public async Task UpdateToolStocktakingPosition(ToolStocktaking toolStocktaking)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                try
                {
                    var foundToolStocktaking = await dbContext.ToolStocktakings.FirstOrDefaultAsync(x => x.ToolStocktakingId == toolStocktaking.ToolStocktakingId);
                    var foundTool = await dbContext.Tools.AsNoTracking().SingleOrDefaultAsync(x => x.ToolId == toolStocktaking.Tool.ToolId);
                    var foundToolLocalization = await dbContext.ToolLocalizations.AsNoTracking().SingleOrDefaultAsync(x => x.ToolLocalizationId == toolStocktaking.Localization.ToolLocalizationId);

                    dbContext.Entry(foundTool).State = EntityState.Unchanged;
                    dbContext.Entry(foundToolLocalization).State = EntityState.Unchanged;

                    dbContext.ToolStocktakings.Attach(foundToolStocktaking);
                    dbContext.Tools.Attach(foundTool);
                    dbContext.ToolLocalizations.Attach(foundToolLocalization);

                    foundToolStocktaking.Localization = foundToolLocalization;
                    foundToolStocktaking.Tool = foundTool;

                    dbContext.Entry(foundToolStocktaking).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                }
                catch (System.Exception exception)
                {
                    throw exception;
                }
            }
        }

        public async Task DeleteToolStocktakingPosition(ToolStocktaking toolStocktaking)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                var foundToolStocktaking = await dbContext.ToolStocktakings.FirstOrDefaultAsync(x => x.ToolStocktakingId == toolStocktaking.ToolStocktakingId);

                dbContext.Entry(foundToolStocktaking).State = EntityState.Deleted;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAllTable()
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                await dbContext.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE ToolStocktakings");
            }
        }

        public async Task ConfirmStocktaking()
        {
            var toolStocktakings = new List<ToolStocktaking>();

            using (var dbContext = new GeoMuzeumContext())
            {
                toolStocktakings = await dbContext.ToolStocktakings.AsNoTracking().Include(x => x.Localization).Include(x => x.Tool).Include(x => x.Tool).ToListAsync();

                await dbContext.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE ToolStocktakings");
            }

            try
            {
                foreach (var toolStocktaking in toolStocktakings)
                {
                    using (var dbContext = new GeoMuzeumContext())
                    {
                        var foundTool = await dbContext.Tools.FindAsync(toolStocktaking.Tool.ToolId);
                        var foundToolLocalization = await dbContext.ToolLocalizations.FindAsync(toolStocktaking.Localization.ToolLocalizationId);

                        dbContext.Entry(foundToolLocalization).State = EntityState.Unchanged;

                        dbContext.Tools.Attach(foundTool);
                        dbContext.ToolLocalizations.Attach(foundToolLocalization);

                        foundTool.Localization = foundToolLocalization;

                        await dbContext.SaveChangesAsync();
                    }
                }
            }
            catch (System.Exception exception)
            {
                throw exception;
            }
        }

        public async Task<List<ToolStocktaking>> GetAllToolsStocktakingsByTool(string toolName)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.ToolStocktakings.AsNoTracking().Include(x => x.Localization).Include(x => x.Tool).Where(x => x.Tool.ToolName.ToLower().Contains(toolName.ToLower())).ToListAsync();
            }
        }

        public async Task<List<ToolStocktaking>> GetAllToolsStocktakingsByLocalization(string localization)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.ToolStocktakings.AsNoTracking().Include(x => x.Localization).Include(x => x.Tool).Where(x => x.Localization.ToolLocalizationNumber.ToLower().Contains(localization.ToLower())).ToListAsync();
            }
        }
    }
}
