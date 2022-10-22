using System.Collections.Generic;

namespace GeoMuzeum.Model
{
    public class ToolLocalization
    {
        public ToolLocalization()
        {
            Tools = new List<Tool>();
        }

        public ToolLocalization(int toolLocalizationId, string toolLocalizationNumber, string toolLocalizationDescription, List<Tool> tools)
        {
            ToolLocalizationId = toolLocalizationId;
            ToolLocalizationNumber = toolLocalizationNumber;
            ToolLocalizationDescription = toolLocalizationDescription;
            Tools = tools;
        }

        public int ToolLocalizationId { get; set; }
        public string ToolLocalizationNumber { get; set; }
        public string ToolLocalizationDescription { get; set; }
        public List<Tool> Tools { get; set; }
    }
}
