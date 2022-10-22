namespace GeoMuzeum.Model
{
    public class Settings
    {
        public Settings()
        {

        }

        public Settings(int settingsId, bool isExhibitStocktaking, bool isToolStocktaking)
        {
            SettingsId = settingsId;
            IsExhibitStocktaking = isExhibitStocktaking;
            IsToolStocktaking = isToolStocktaking;
        }

        public int SettingsId { get; set; }
        public bool IsExhibitStocktaking { get; set; }
        public bool IsToolStocktaking { get; set; }
    }
}
