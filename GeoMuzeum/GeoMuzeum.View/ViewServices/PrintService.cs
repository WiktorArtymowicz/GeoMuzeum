using GeoMuzeum.Model;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;

namespace GeoMuzeum.View.ViewServices
{
    public static class PrintService
    {
        public static void PrintExhibits(List<Exhibit> exhibits)
        {
            var flowDocument = CreateFlowDocument();

            flowDocument.Blocks.Add(CreateFirstParagraph("Lista eksponatów"));
            flowDocument.Name = "ListaEksponatów";

            flowDocument.Blocks.Add(CreateDescriptionParagraph($" "));
            flowDocument.Blocks.Add(CreateDescriptionParagraph($"Data wydruku: {DateTime.Now.ToLongDateString()}"));
            flowDocument.Blocks.Add(CreateDescriptionParagraph($" "));

            foreach (var exhibit in exhibits)
            {
                Section section = new Section();

                section.Blocks.Add(CreateDescriptionParagraph("Eksponat: "));
                section.Blocks.Add(new Paragraph(new Run($"Nazwa: {exhibit.ExhibitName}")));
                section.Blocks.Add(new Paragraph(new Run($"Katalog: {exhibit.Catalog.CatalogName}")));
                section.Blocks.Add(new Paragraph(new Run($"Lokalizacja: {exhibit.Localization.ExhibitLocalizationNumber}")));
                section.Blocks.Add(new Paragraph(new Run($"Typ: {exhibit.ExhibitType}")));
                section.Blocks.Add(new Paragraph(new Run($"Opis: {exhibit.ExhibitDescription}")));

                flowDocument.Blocks.Add(section);
            }

            ShowPrintDialog(flowDocument);
        }

        public static void PrintExhibitStocktakings(List<ExhibitStocktaking> exhibitStocktakings)
        {
            var flowDocument = CreateFlowDocument();
            flowDocument.Name = "RemanentEksponatów";

            flowDocument.Blocks.Add(CreateFirstParagraph("Remanent eksponatów"));
            flowDocument.Blocks.Add(CreateDescriptionParagraph($"Data wydruku: {DateTime.Now.ToShortDateString()}"));
            flowDocument.Blocks.Add(CreateDescriptionParagraph($" "));

            foreach (var exhibitStocktaking in exhibitStocktakings)
            {
                Section section = new Section();

                section.Blocks.Add(CreateDescriptionParagraph("Eksponat: "));
                section.Blocks.Add(new Paragraph(new Run($"Nazwa: {exhibitStocktaking.Exhibit.ExhibitName}")));
                section.Blocks.Add(new Paragraph(new Run($"Katalog: {exhibitStocktaking.Catalog.CatalogName}")));
                section.Blocks.Add(new Paragraph(new Run($"Lokalizacja: {exhibitStocktaking.Localization.ExhibitLocalizationNumber}")));

                flowDocument.Blocks.Add(section);
            }

            ShowPrintDialog(flowDocument);
        }

        public static void PrintToolStocktakings(List<ToolStocktaking> toolStocktakings)
        {
            var flowDocument = CreateFlowDocument();
            flowDocument.Name = "RemanentNarzędzi";

            flowDocument.Blocks.Add(CreateFirstParagraph("Remanent narzędzi"));
            flowDocument.Blocks.Add(CreateDescriptionParagraph($"Data wydruku: {DateTime.Now.ToShortDateString()}"));
            flowDocument.Blocks.Add(CreateDescriptionParagraph($" "));

            foreach (var toolStocktaking in toolStocktakings)
            {
                Section section = new Section();

                section.Blocks.Add(CreateDescriptionParagraph("Narzędzie: "));
                section.Blocks.Add(new Paragraph(new Run($"Nazwa: {toolStocktaking.Tool.ToolName}")));
                section.Blocks.Add(new Paragraph(new Run($"Lokalizacja: {toolStocktaking.Localization.ToolLocalizationNumber}")));

                flowDocument.Blocks.Add(section);
            }

            ShowPrintDialog(flowDocument);
        }

        private static FlowDocument CreateFlowDocument()
        {
            FlowDocument doc = new FlowDocument();
            doc.MaxPageWidth = 512.0;
            doc.MaxPageHeight = 750.0;

            return doc;
        }

        private static Paragraph CreateFirstParagraph(string text)
        {
            Paragraph paragraph = new Paragraph();

            Bold bold = new Bold();
            bold.Inlines.Add(new Run(text));

            //Italic italicBlod = new Italic();
            //italicBlod.Inlines.Add(bold);

            paragraph.Inlines.Add(bold);

            paragraph.TextAlignment = System.Windows.TextAlignment.Center;
            paragraph.FontSize = 24;

            return paragraph;
        }

        private static Paragraph CreateDescriptionParagraph(string text)
        {
            Paragraph paragraph = new Paragraph();

            Bold bold = new Bold();
            bold.Inlines.Add(new Run(text));

            Italic italicBlod = new Italic();
            italicBlod.Inlines.Add(bold);

            paragraph.Inlines.Add(bold);

            return paragraph;
        }

        private static void ShowPrintDialog(FlowDocument flowDocument)
        {
            PrintDialog printDlg = new PrintDialog();

            if (printDlg.ShowDialog() == true)
            {
                IDocumentPaginatorSource idpSource = flowDocument;
                printDlg.PrintDocument(idpSource.DocumentPaginator, flowDocument.Name);
            }
        }
    }
}
