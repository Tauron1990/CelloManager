using System.Globalization;
using System.Threading.Tasks;
using CelloManager.Core.Data;
using CelloManager.Core.Printing.Workflow;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace CelloManager.Core.Printing.Steps;

public sealed class PrerenderImages : PrinterStep
{
    public override async ValueTask<StepId> OnExecute(PrinterContext context)
    {
        await context.PrintUiModel.Init(context).ConfigureAwait(false);
        
        var model = context.PrintUiModel;
        // ReSharper disable AccessToDisposedClosure
        var document = Document.Create(
            c =>
            {
                foreach (var page in model)
                {
                    c.Page(
                        pd =>
                        {
                            pd.Margin(0.5f, Unit.Centimetre);
                            
                            CreateHeader(pd, page);
                            
                            pd.Content().Column(cd => TableBuilder(page, cd));
                            
                            CreateFooter(pd);
                        });
                }
            });
        // ReSharper restore AccessToDisposedClosure
        context.Document = document;
        
        return StepId.None;
    }

    private static void TableBuilder(PendingOrder page, ColumnDescriptor cd)
    {
        foreach (var spoolList in page.Spools)
        {
            cd.Item().Table(
                td =>
                {
                    td.Header(tcd =>
                              {
                                  tcd.Cell().RowSpan(2).Text(spoolList.Category);
                                  tcd.Cell().Text("Name");
                                  tcd.Cell().Text("Menge");
                              });
                    td.ColumnsDefinition(tcdd =>
                                         {
                                             tcdd.RelativeColumn();
                                             tcdd.RelativeColumn();
                                         });

                    foreach (var spool in spoolList.Spools)
                    {
                        td.Cell().Text(spool.Name);
                        td.Cell().Text(spool.Amount.ToString(CultureInfo.CurrentUICulture));
                    }
                });
        }
    }

    private static void CreateFooter(PageDescriptor pd)
    {
        pd.Footer().Column(
            cd =>
            {
                cd.Item().Text(
                    td =>
                    {
                        td.Span("Seite: ");
                        td.CurrentPageNumber();
                    });
            });
    }

    private static void CreateHeader(PageDescriptor pd, PendingOrder page)
    {
        pd.Header().Column(
            cd =>
            {
                cd.Item().Text(
                    td => { td.Span($"Bestellung: {page.Id} - {page.Time:D}").Bold().FontSize(15); });
            });
    }
}