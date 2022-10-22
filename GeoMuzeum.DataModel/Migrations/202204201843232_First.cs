namespace GeoMuzeum.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class First : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Catalogs",
                c => new
                    {
                        CatalogId = c.Int(nullable: false, identity: true),
                        CatalogName = c.String(),
                        CatalogDescription = c.String(),
                        User_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.CatalogId)
                .ForeignKey("dbo.Users", t => t.User_UserId)
                .Index(t => t.User_UserId);
            
            CreateTable(
                "dbo.Exhibits",
                c => new
                    {
                        ExhibitId = c.Int(nullable: false, identity: true),
                        ExhibitName = c.String(),
                        ExhibitDescription = c.String(),
                        ExhibitType = c.Int(nullable: false),
                        Catalog_CatalogId = c.Int(),
                        Localization_ExhibitLocalizationId = c.Int(),
                    })
                .PrimaryKey(t => t.ExhibitId)
                .ForeignKey("dbo.Catalogs", t => t.Catalog_CatalogId)
                .ForeignKey("dbo.ExhibitLocalizations", t => t.Localization_ExhibitLocalizationId)
                .Index(t => t.Catalog_CatalogId)
                .Index(t => t.Localization_ExhibitLocalizationId);
            
            CreateTable(
                "dbo.ExhibitLocalizations",
                c => new
                    {
                        ExhibitLocalizationId = c.Int(nullable: false, identity: true),
                        ExhibitLocalizationNumber = c.String(),
                        ExhibitLocalizationDescription = c.String(),
                    })
                .PrimaryKey(t => t.ExhibitLocalizationId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        UserSurname = c.String(),
                        UserPosition = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.ExhibitStocktakings",
                c => new
                    {
                        ExhibitStocktakingId = c.Int(nullable: false, identity: true),
                        Catalog_CatalogId = c.Int(),
                        Exhibit_ExhibitId = c.Int(),
                        Localization_ExhibitLocalizationId = c.Int(),
                    })
                .PrimaryKey(t => t.ExhibitStocktakingId)
                .ForeignKey("dbo.Catalogs", t => t.Catalog_CatalogId)
                .ForeignKey("dbo.Exhibits", t => t.Exhibit_ExhibitId)
                .ForeignKey("dbo.ExhibitLocalizations", t => t.Localization_ExhibitLocalizationId)
                .Index(t => t.Catalog_CatalogId)
                .Index(t => t.Exhibit_ExhibitId)
                .Index(t => t.Localization_ExhibitLocalizationId);
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        SettingsId = c.Int(nullable: false, identity: true),
                        IsExhibitStocktaking = c.Boolean(nullable: false),
                        IsToolStocktaking = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.SettingsId);
            
            CreateTable(
                "dbo.ToolLocalizations",
                c => new
                    {
                        ToolLocalizationId = c.Int(nullable: false, identity: true),
                        ToolLocalizationNumber = c.String(),
                        ToolLocalizationDescription = c.String(),
                    })
                .PrimaryKey(t => t.ToolLocalizationId);
            
            CreateTable(
                "dbo.Tools",
                c => new
                    {
                        ToolId = c.Int(nullable: false, identity: true),
                        ToolName = c.String(),
                        ToolDescription = c.String(),
                        Localization_ToolLocalizationId = c.Int(),
                    })
                .PrimaryKey(t => t.ToolId)
                .ForeignKey("dbo.ToolLocalizations", t => t.Localization_ToolLocalizationId)
                .Index(t => t.Localization_ToolLocalizationId);
            
            CreateTable(
                "dbo.ToolStocktakings",
                c => new
                    {
                        ToolStocktakingId = c.Int(nullable: false, identity: true),
                        Localization_ToolLocalizationId = c.Int(),
                        Tool_ToolId = c.Int(),
                    })
                .PrimaryKey(t => t.ToolStocktakingId)
                .ForeignKey("dbo.ToolLocalizations", t => t.Localization_ToolLocalizationId)
                .ForeignKey("dbo.Tools", t => t.Tool_ToolId)
                .Index(t => t.Localization_ToolLocalizationId)
                .Index(t => t.Tool_ToolId);
            
            CreateTable(
                "dbo.UserLogins",
                c => new
                    {
                        UserLoginId = c.Int(nullable: false, identity: true),
                        Login = c.String(),
                        PinNumber = c.Int(nullable: false),
                        User_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserLoginId)
                .ForeignKey("dbo.Users", t => t.User_UserId, cascadeDelete: true)
                .Index(t => t.User_UserId);
            
            CreateTable(
                "dbo.UserLogs",
                c => new
                    {
                        UserLogId = c.Int(nullable: false, identity: true),
                        LogDescription = c.String(),
                        User_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.UserLogId)
                .ForeignKey("dbo.Users", t => t.User_UserId)
                .Index(t => t.User_UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserLogs", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.UserLogins", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.ToolStocktakings", "Tool_ToolId", "dbo.Tools");
            DropForeignKey("dbo.ToolStocktakings", "Localization_ToolLocalizationId", "dbo.ToolLocalizations");
            DropForeignKey("dbo.Tools", "Localization_ToolLocalizationId", "dbo.ToolLocalizations");
            DropForeignKey("dbo.ExhibitStocktakings", "Localization_ExhibitLocalizationId", "dbo.ExhibitLocalizations");
            DropForeignKey("dbo.ExhibitStocktakings", "Exhibit_ExhibitId", "dbo.Exhibits");
            DropForeignKey("dbo.ExhibitStocktakings", "Catalog_CatalogId", "dbo.Catalogs");
            DropForeignKey("dbo.Catalogs", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.Exhibits", "Localization_ExhibitLocalizationId", "dbo.ExhibitLocalizations");
            DropForeignKey("dbo.Exhibits", "Catalog_CatalogId", "dbo.Catalogs");
            DropIndex("dbo.UserLogs", new[] { "User_UserId" });
            DropIndex("dbo.UserLogins", new[] { "User_UserId" });
            DropIndex("dbo.ToolStocktakings", new[] { "Tool_ToolId" });
            DropIndex("dbo.ToolStocktakings", new[] { "Localization_ToolLocalizationId" });
            DropIndex("dbo.Tools", new[] { "Localization_ToolLocalizationId" });
            DropIndex("dbo.ExhibitStocktakings", new[] { "Localization_ExhibitLocalizationId" });
            DropIndex("dbo.ExhibitStocktakings", new[] { "Exhibit_ExhibitId" });
            DropIndex("dbo.ExhibitStocktakings", new[] { "Catalog_CatalogId" });
            DropIndex("dbo.Exhibits", new[] { "Localization_ExhibitLocalizationId" });
            DropIndex("dbo.Exhibits", new[] { "Catalog_CatalogId" });
            DropIndex("dbo.Catalogs", new[] { "User_UserId" });
            DropTable("dbo.UserLogs");
            DropTable("dbo.UserLogins");
            DropTable("dbo.ToolStocktakings");
            DropTable("dbo.Tools");
            DropTable("dbo.ToolLocalizations");
            DropTable("dbo.Settings");
            DropTable("dbo.ExhibitStocktakings");
            DropTable("dbo.Users");
            DropTable("dbo.ExhibitLocalizations");
            DropTable("dbo.Exhibits");
            DropTable("dbo.Catalogs");
        }
    }
}
