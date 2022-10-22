using System.ComponentModel.DataAnnotations;

namespace GeoMuzeum.Model
{
    public class ToolStocktaking
    {
        public ToolStocktaking()
        {

        }

        public ToolStocktaking(int toolStocktakingId, Tool tool, ToolLocalization localization)
        {
            ToolStocktakingId = toolStocktakingId;
            Tool = tool;
            Localization = localization;
        }

        public int ToolStocktakingId { get; set; }
        public virtual Tool Tool { get; set; }
        public virtual ToolLocalization Localization { get; set; }
    }
}
