using GeoMuzeum.Model.Enums;

namespace GeoMuzeum.Model
{
    public class Exhibit
    {
        public Exhibit()
        {

        }

        public Exhibit(int exhibitId, string exhibitName, string exhibitDescription, ExhibitType exhibitType, Catalog catalog, ExhibitLocalization localization)
        {
            ExhibitId = exhibitId;
            ExhibitName = exhibitName;
            ExhibitDescription = exhibitDescription;
            ExhibitType = exhibitType;
            Catalog = catalog;
            Localization = localization;
        }

        public int ExhibitId { get; set; }
        public string ExhibitName { get; set; }
        public string ExhibitDescription { get; set; }
        public ExhibitType ExhibitType { get; set; }
        public virtual Catalog Catalog { get; set; }
        public virtual ExhibitLocalization Localization { get; set; }

        public string ExhibitTypeInfo => ExhibitType.ToString();

        public string ExhibitInfo => ToString();

        public override string ToString()
        {
            return $"Eksponat: {ExhibitName}, {ExhibitType},  {(Catalog == null ? string.Empty : Catalog.CatalogName)}, {(Localization == null ? string.Empty : Localization.ExhibitLocalizationNumber)}";
        }
    }
}
