namespace GeoMuzeum.Model
{
    public class Tool 
    {
        public Tool()
        {

        }

        public Tool(int toolId, string toolName, string toolDescription, ToolLocalization localization)
        {
            ToolId = toolId;
            ToolName = toolName;
            ToolDescription = toolDescription;
            Localization = localization;
        }

        public int ToolId { get; set; }
        public string ToolName { get; set; }
        public string ToolDescription { get; set; }
        public virtual ToolLocalization Localization { get; set; }
    }
}
