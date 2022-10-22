using GeoMuzeum.Model;
using System.Collections.Generic;
using System.Data.Entity;

namespace GeoMuzeum.DataModel
{
    public class GeoMuzeumContext : DbContext
    {
        public GeoMuzeumContext() : base("MyConnectionString")
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<Exhibit> Exhibits { get; set; }
        public DbSet<ExhibitStocktaking> ExhibitStocktakings { get; set; }
        public DbSet<ExhibitLocalization> ExhibitLocalizations { get; set; }
        public DbSet<ToolLocalization> ToolLocalizations { get; set; }
        public DbSet<Tool> Tools { get; set; }
        public DbSet<ToolStocktaking> ToolStocktakings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<Settings> Settings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }
    }
}
