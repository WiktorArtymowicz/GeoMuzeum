using System.Collections.Generic;

namespace GeoMuzeum.Model
{
    public class ExhibitLocalization
    {
        public ExhibitLocalization()
        {
            Exhibits = new List<Exhibit>();
        }

        public ExhibitLocalization(int exhibitLocalizationId, string exhibitLocalizationNumber, string exhibitLocalizationDescription, List<Exhibit> exhibits)
        {
            ExhibitLocalizationId = exhibitLocalizationId;
            ExhibitLocalizationNumber = exhibitLocalizationNumber;
            ExhibitLocalizationDescription = exhibitLocalizationDescription;
            Exhibits = exhibits;
        }

        public int ExhibitLocalizationId { get; set; }
        public string ExhibitLocalizationNumber { get; set; }
        public string ExhibitLocalizationDescription { get; set; }
        public List<Exhibit> Exhibits { get; set; }
    }
}
