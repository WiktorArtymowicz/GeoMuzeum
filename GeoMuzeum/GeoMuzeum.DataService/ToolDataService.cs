using GeoMuzeum.DataModel;
using GeoMuzeum.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public class ToolDataService : IToolDataService
    {
        public async Task<List<Tool>> GetAllTools()
        {
            using (var dbContext = new GeoMuzeumContext())
            { 
                return await dbContext.Tools.AsNoTracking().Include(x => x.Localization).ToListAsync();
            }
        }

        public async Task<List<Tool>> GetToolsByName(string name)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                if (!await dbContext.Tools.AnyAsync())
                    return new List<Tool>();

                return await dbContext.Tools.AsNoTracking().Where(x => x.ToolName.ToLower().Contains(name.ToLower())).Include(x => x.Localization).ToListAsync();
            }
        }

        public async Task<List<Tool>> GetToolsByInfo(string info)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                if (!await dbContext.Tools.AnyAsync())
                    return new List<Tool>();

                return await dbContext.Tools.AsNoTracking().Where(x => x.ToolDescription.ToLower().Contains(info.ToLower())).Include(x => x.Localization).ToListAsync();
            }
        }

        public async Task<List<Tool>> GetToolsByLocalization(string lozalization)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                if (!await dbContext.Tools.AnyAsync())
                    return new List<Tool>();

                return await dbContext.Tools.AsNoTracking().Where(x => x.Localization.ToolLocalizationNumber.ToLower().Contains(lozalization.ToLower())).Include(x => x.Localization).ToListAsync();
            }
        }

        public async Task<Tool> GetToolById(Tool tool)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.Tools.AsNoTracking().FirstOrDefaultAsync(x => x.ToolId == tool.ToolId);
            }
        }

        public async Task AddNewTool(Tool tool)
        {
            try
            {
                using (var dbContext = new GeoMuzeumContext())
                {
                    var foundToolLocalization = await dbContext.ToolLocalizations.FindAsync(tool.Localization.ToolLocalizationId);

                    dbContext.Entry(foundToolLocalization).State = EntityState.Unchanged;
                    dbContext.ToolLocalizations.Attach(foundToolLocalization);

                    tool.Localization = foundToolLocalization;

                    dbContext.Tools.Add(tool);
                    dbContext.Entry(tool).State = EntityState.Added;
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (System.Exception exception)
            {
                throw exception;
            }
        }

        public async Task UpdateTool(Tool tool)
        {
            try
            {
                using (var dbContext = new GeoMuzeumContext())
                {
                    var foundTool = await dbContext.Tools.FindAsync(tool.ToolId);
                    var foundToolLocalization = await dbContext.ToolLocalizations.FindAsync(tool.Localization.ToolLocalizationId);

                    dbContext.Entry(foundToolLocalization).State = EntityState.Unchanged;

                    dbContext.Tools.Attach(foundTool);
                    dbContext.ToolLocalizations.Attach(foundToolLocalization);

                    foundTool.ToolName = tool.ToolName;
                    foundTool.ToolDescription = tool.ToolDescription;
                    foundTool.Localization = foundToolLocalization;

                    await dbContext.SaveChangesAsync();
                }
            }
            catch (System.Exception exception)
            {
                throw exception;
            }
        }

        public async Task DeleteTool(Tool tool)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                var foundTool = await dbContext.Tools.FindAsync(tool.ToolId);

                dbContext.Entry(foundTool).State = EntityState.Deleted;
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
