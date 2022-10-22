
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GeoMuzeum.Model
{
    public class Catalog
    {
        public Catalog()
        {
            Exhibits = new List<Exhibit>();
        }

        public Catalog(int catalogId, string catalogName, string catalogDescription, List<Exhibit> localizationItems, User user)
        {
            CatalogId = catalogId;
            CatalogName = catalogName;
            CatalogDescription = catalogDescription;
            Exhibits = localizationItems;
            User = user;
        }

        public int CatalogId { get; set; }
        public string CatalogName { get; set; }
        public string CatalogDescription { get; set; }
        public List<Exhibit> Exhibits { get; set; }
        public User User { get; set; }
    }
}
