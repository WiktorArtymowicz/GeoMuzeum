using System.ComponentModel.DataAnnotations;

namespace GeoMuzeum.Model
{
    public class ExhibitStocktaking
    {
        public ExhibitStocktaking()
        {

        }

        public ExhibitStocktaking(int exhibitStocktakingId, Exhibit exhibit, ExhibitLocalization localization, Catalog catalog)
        {
            ExhibitStocktakingId = exhibitStocktakingId;
            Exhibit = exhibit;
            Localization = localization;
            Catalog = catalog;
        }

        public int ExhibitStocktakingId { get; set; }
        public virtual Exhibit Exhibit { get; set; }
        public virtual ExhibitLocalization Localization { get; set; }
        public virtual Catalog Catalog { get; set; }
    }
}
