namespace GeoMuzeum.DataModel.Migrations
{
    using GeoMuzeum.Model;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<GeoMuzeum.DataModel.GeoMuzeumContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(GeoMuzeum.DataModel.GeoMuzeumContext context)
        {
            var admin = new User { UserName = "Admin", UserSurname = "Admin", UserPosition = Model.Enums.UserPosition.Admin };
            var anna = new User { UserName = "Anna", UserSurname = "Artymowicz", UserPosition = Model.Enums.UserPosition.Kierownik };
            var magda = new User { UserName = "Magda", UserSurname = "Magda", UserPosition = Model.Enums.UserPosition.Pracownik };

            context.Users.AddOrUpdate(x => x.UserId, admin);
            context.Users.AddOrUpdate(x => x.UserId, anna);
            context.Users.AddOrUpdate(x => x.UserId, magda);

            context.UserLogins.AddOrUpdate(x => x.UserLoginId, new UserLogin { Login = "Admin", PinNumber = 1234, User = admin });
            context.UserLogins.AddOrUpdate(x => x.UserLoginId, new UserLogin { Login = "Anna", PinNumber = 5678, User = anna });
            context.UserLogins.AddOrUpdate(x => x.UserLoginId, new UserLogin { Login = "Magda", PinNumber = 1111, User = magda });

            var catalog1 = new Catalog { CatalogName = "Domyślny katalog", CatalogDescription = "Katalog utworzony domyślnie przez system", User = admin };
            var catalog2 = new Catalog { CatalogName = "Katalog nr. 1", CatalogDescription = "Katalog Nr. 1", User = anna };
            var catalog3 = new Catalog { CatalogName = "Katalog nr. 2", CatalogDescription = "Katalog Nr. 2", User = anna };
            var catalog4 = new Catalog { CatalogName = "Katalog nr. 3", CatalogDescription = "Katalog Nr. 3", User = anna };
            var catalog5 = new Catalog { CatalogName = "Katalog nr. 4", CatalogDescription = "Katalog Nr. 4", User = anna };

            context.Catalogs.AddOrUpdate(x => x.CatalogId, catalog1);
            context.Catalogs.AddOrUpdate(x => x.CatalogId, catalog2);
            context.Catalogs.AddOrUpdate(x => x.CatalogId, catalog3);
            context.Catalogs.AddOrUpdate(x => x.CatalogId, catalog4);
            context.Catalogs.AddOrUpdate(x => x.CatalogId, catalog5);

            var localization = new ExhibitLocalization { ExhibitLocalizationNumber = "Domyślna - Eksponat", ExhibitLocalizationDescription = "Lokalizacja domyślnie utworzona przez system." };
            var localization2 = new ExhibitLocalization { ExhibitLocalizationNumber = "Gablota 1", ExhibitLocalizationDescription = "Największa gablota." };
            var localization3 = new ExhibitLocalization { ExhibitLocalizationNumber = "Gablota Minerały", ExhibitLocalizationDescription = "Gablota na minerały." };
            var localization4 = new ExhibitLocalization { ExhibitLocalizationNumber = "Gablota Skamieniałości", ExhibitLocalizationDescription = "Gablota na skamieniałości." };
            var localization5 = new ExhibitLocalization { ExhibitLocalizationNumber = "Gablota koło biurka", ExhibitLocalizationDescription = "Gablota koło biurka." };
            var localization6 = new ExhibitLocalization { ExhibitLocalizationNumber = "Górna półka", ExhibitLocalizationDescription = "Półka nad gablotą." };
            var localization7 = new ExhibitLocalization { ExhibitLocalizationNumber = "Gablota koło drzwi", ExhibitLocalizationDescription = "Gablota koło drzwi." };
            var localization8 = new ExhibitLocalization { ExhibitLocalizationNumber = "Galbota - korytarz 1", ExhibitLocalizationDescription = "Pierwsza gablota na korytarzu." };
            var localization9 = new ExhibitLocalization { ExhibitLocalizationNumber = "Galbota - korytarz 2", ExhibitLocalizationDescription = "Druga gablota na korytarzu." };
            var localization10 = new ExhibitLocalization { ExhibitLocalizationNumber = "Galbota - korytarz 3", ExhibitLocalizationDescription = "Trzecia gablota na korytarzu." };

            context.ExhibitLocalizations.AddOrUpdate(x => x.ExhibitLocalizationId, localization);
            context.ExhibitLocalizations.AddOrUpdate(x => x.ExhibitLocalizationId, localization2);
            context.ExhibitLocalizations.AddOrUpdate(x => x.ExhibitLocalizationId, localization3);
            context.ExhibitLocalizations.AddOrUpdate(x => x.ExhibitLocalizationId, localization4);
            context.ExhibitLocalizations.AddOrUpdate(x => x.ExhibitLocalizationId, localization5);
            context.ExhibitLocalizations.AddOrUpdate(x => x.ExhibitLocalizationId, localization6);
            context.ExhibitLocalizations.AddOrUpdate(x => x.ExhibitLocalizationId, localization7);
            context.ExhibitLocalizations.AddOrUpdate(x => x.ExhibitLocalizationId, localization8);
            context.ExhibitLocalizations.AddOrUpdate(x => x.ExhibitLocalizationId, localization9);
            context.ExhibitLocalizations.AddOrUpdate(x => x.ExhibitLocalizationId, localization10);

            var exhibit1 = new Exhibit { ExhibitName = "Agat", ExhibitDescription = "Agat znaleziony we Włoszech", ExhibitType = Model.Enums.ExhibitType.Minerał, Localization = localization, Catalog = catalog1 };
            var exhibit2 = new Exhibit { ExhibitName = "Marmur", ExhibitDescription = "Marmur znaleziony w miejscowości Carrara w Hiszpani", ExhibitType = Model.Enums.ExhibitType.Skała, Localization = localization2, Catalog = catalog2 };
            var exhibit3 = new Exhibit { ExhibitName = "Pałygorskit", ExhibitDescription = "Rędziny, Dolnośląśkie, Polska", ExhibitType = Model.Enums.ExhibitType.Minerał, Localization = localization3, Catalog = catalog3 };
            var exhibit4 = new Exhibit { ExhibitName = "Gnejs", ExhibitDescription = "Acarta River, Kanada", ExhibitType = Model.Enums.ExhibitType.Skała, Localization = localization4, Catalog = catalog4 };
            var exhibit5 = new Exhibit { ExhibitName = "Granit Rapakivi", ExhibitDescription = "Skaleń alkaliczny z obwódkami oligoklazu. Vehkalahti, Summe, Finlandia", ExhibitType = Model.Enums.ExhibitType.Skała, Localization = localization5, Catalog = catalog5 };
            var exhibit6 = new Exhibit { ExhibitName = "Komatyt", ExhibitDescription = "Australia Zachodnia", ExhibitType = Model.Enums.ExhibitType.Skała, Localization = localization6, Catalog = catalog1 };
            var exhibit7 = new Exhibit { ExhibitName = "Pegmatyt", ExhibitDescription = "Tekstura pismowa (kwarcu i skalenia). Norwegia", ExhibitType = Model.Enums.ExhibitType.Skała, Localization = localization7, Catalog = catalog2 };
            var exhibit8 = new Exhibit { ExhibitName = "Dolomit", ExhibitDescription = "Jóra Górna. Księża Góra, Kraków, Polska", ExhibitType = Model.Enums.ExhibitType.Skała, Localization = localization8, Catalog = catalog3 };
            var exhibit9 = new Exhibit { ExhibitName = "Bentonit", ExhibitDescription = "Warstwy porębskie. kop. Czerwona Gwardia GZW, Polska", ExhibitType = Model.Enums.ExhibitType.Skała, Localization = localization9, Catalog = catalog4 };
            var exhibit10 = new Exhibit { ExhibitName = "Stromatolit", ExhibitDescription = "Kambr/Prekambr. Quarzazate, Anty-Atlas, Maroko", ExhibitType = Model.Enums.ExhibitType.Skamieniałość, Localization = localization10, Catalog = catalog5 };
            var exhibit11 = new Exhibit { ExhibitName = "Trylobit", ExhibitDescription = "Ordowik. Putilowo, Rosja", ExhibitType = Model.Enums.ExhibitType.Skamieniałość, Localization = localization, Catalog = catalog1 };
            var exhibit12 = new Exhibit { ExhibitName = "Starorak", ExhibitDescription = "Sylur. Kamieniec Podolski, Ukraina", ExhibitType = Model.Enums.ExhibitType.Skamieniałość, Localization = localization2, Catalog = catalog2 };
            var exhibit13 = new Exhibit { ExhibitName = "Koralowiec", ExhibitDescription = "Dewon. Kowala, G.Świętokrzyskie, Polska", ExhibitType = Model.Enums.ExhibitType.Skamieniałość, Localization = localization3, Catalog = catalog3 };
            var exhibit14 = new Exhibit { ExhibitName = "Paproć", ExhibitDescription = "Karbon. Przedwojów k/ Kamiennej Góry, Polska", ExhibitType = Model.Enums.ExhibitType.Skamieniałość, Localization = localization4, Catalog = catalog4 };
            var exhibit15 = new Exhibit { ExhibitName = "Kwiatostan paproci nasiennych", ExhibitDescription = "Perm. Otowice k. Broumow, Czechy", ExhibitType = Model.Enums.ExhibitType.Skamieniałość, Localization = localization5, Catalog = catalog5 };
            var exhibit16 = new Exhibit { ExhibitName = "Fragment kości udowej", ExhibitDescription = "Trias. Zawiercie - Marciszów, Polska", ExhibitType = Model.Enums.ExhibitType.Skamieniałość, Localization = localization6, Catalog = catalog1 };
            var exhibit17 = new Exhibit { ExhibitName = "Amomit", ExhibitDescription = "Jura. Łuków k. Siedlec, Polska", ExhibitType = Model.Enums.ExhibitType.Skamieniałość, Localization = localization7, Catalog = catalog2 };
            var exhibit18 = new Exhibit { ExhibitName = "Ryba, ząb", ExhibitDescription = "Kreda. Zajęcza Góra k. Buska, Polska", ExhibitType = Model.Enums.ExhibitType.Skamieniałość, Localization = localization8, Catalog = catalog3 };
            var exhibit19 = new Exhibit { ExhibitName = "Miedź rodzima", ExhibitDescription = "Cu. Michigan, USA", ExhibitType = Model.Enums.ExhibitType.Minerał, Localization = localization8, Catalog = catalog3 };
            var exhibit20 = new Exhibit { ExhibitName = "Sfaleryt", ExhibitDescription = "ZnS. kop. Pomorzany, Olkusz, Polska", ExhibitType = Model.Enums.ExhibitType.Minerał, Localization = localization9, Catalog = catalog4 };
            var exhibit21 = new Exhibit { ExhibitName = "Halit", ExhibitDescription = "NaCl. kop. Kłodawa Kujawy, Polska", ExhibitType = Model.Enums.ExhibitType.Minerał, Localization = localization10, Catalog = catalog5 };
            var exhibit22 = new Exhibit { ExhibitName = "Korund", ExhibitDescription = "Al2O3. Indie", ExhibitType = Model.Enums.ExhibitType.Minerał, Localization = localization, Catalog = catalog1 };
            var exhibit23 = new Exhibit { ExhibitName = "Kalcyt", ExhibitDescription = "CaCO3. Turinskoje, Kranojarski Kraj, Rosja", ExhibitType = Model.Enums.ExhibitType.Minerał, Localization = localization2, Catalog = catalog2 };
            var exhibit24 = new Exhibit { ExhibitName = "Gips", ExhibitDescription = "CaSO4 * 2H2O. Isfara, Uzbekistan", ExhibitType = Model.Enums.ExhibitType.Minerał, Localization = localization3, Catalog = catalog3 };
            var exhibit25 = new Exhibit { ExhibitName = "Purpuryt", ExhibitDescription = "Mn3+ PO4. Sandamab, Namibia", ExhibitType = Model.Enums.ExhibitType.Minerał, Localization = localization4, Catalog = catalog4 };
            var exhibit26 = new Exhibit { ExhibitName = "Uwarowit", ExhibitDescription = "Ca3 Cr2 (SiO4)3. Sarany, Ural, Rosja", ExhibitType = Model.Enums.ExhibitType.Minerał, Localization = localization5, Catalog = catalog5 };
            var exhibit27 = new Exhibit { ExhibitName = "Beryl", ExhibitDescription = "Be3 Al2 (Si6O18). Gilgit, Pakistan", ExhibitType = Model.Enums.ExhibitType.Minerał, Localization = localization6, Catalog = catalog1 };
            var exhibit28 = new Exhibit { ExhibitName = "Opal", ExhibitDescription = "SiO2 * nH2O. Queensland, Australia", ExhibitType = Model.Enums.ExhibitType.Minerał, Localization = localization7, Catalog = catalog2 };

            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit1);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit2);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit3);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit4);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit5);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit6);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit7);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit8);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit9);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit10);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit11);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit12);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit13);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit14);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit15);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit16);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit17);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit18);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit19);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit20);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit21);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit22);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit23);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit24);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit25);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit26);
            context.Exhibits.AddOrUpdate(x => x.ExhibitId, exhibit27);

            var toolLocalization1 = new ToolLocalization { ToolLocalizationNumber = "Domyślna - Narzędzie", ToolLocalizationDescription = "Lokalizacja domyślnie utworzona przez system." };
            var toolLocalization2 = new ToolLocalization { ToolLocalizationNumber = "Narzędzia - Szafka", ToolLocalizationDescription = "Szafka" };
            var toolLocalization3 = new ToolLocalization { ToolLocalizationNumber = "Narzędzia - Składzik", ToolLocalizationDescription = "Składzik" };

            context.ToolLocalizations.AddOrUpdate(x => x.ToolLocalizationId, toolLocalization1);
            context.ToolLocalizations.AddOrUpdate(x => x.ToolLocalizationId, toolLocalization2);
            context.ToolLocalizations.AddOrUpdate(x => x.ToolLocalizationId, toolLocalization3);

            var tool1 = new Tool { ToolName = "Młotek", ToolDescription = "Młotek geologiczny", Localization = toolLocalization1 };
            var tool2 = new Tool { ToolName = "Łopata", ToolDescription = "Łopata do ogrodu", Localization = toolLocalization2 };
            var tool3 = new Tool { ToolName = "Grabie", ToolDescription = "Grabie do ogrodu", Localization = toolLocalization3 };

            context.Tools.AddOrUpdate(x => x.ToolId, tool1);
            context.Tools.AddOrUpdate(x => x.ToolId, tool2);
            context.Tools.AddOrUpdate(x => x.ToolId, tool3);
        }
    }
}
