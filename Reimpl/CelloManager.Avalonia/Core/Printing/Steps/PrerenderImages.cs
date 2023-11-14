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
                            pd.DefaultTextStyle(ts => ts.Bold().FontSize(15));
                            pd.Margin(0.7f, Unit.Centimetre);
                            
                            CreateHeader(pd, page);
                            
                            TableBuilder(page, pd.Content());
                            
                            CreateFooter(pd);
                        });
                }
            });
        // ReSharper restore AccessToDisposedClosure
        context.Document = document;
        
        return StepId.None;
    }

    private static void TableBuilder(PendingOrder page, IContainer container)
    {
        bool lineSet = false;
        
        container.Row(
            cd =>
            {
                foreach (var spoolList in page.Spools)
                {
                    cd.RelativeItem().Table(
                        td =>
                        {
                            td.Header(tcd =>
                                      {
                                          tcd.Cell().ColumnSpan(2).Padding(3).Text(spoolList.Category);
                                          tcd.Cell().Padding(3).Text("Name");
                                          tcd.Cell().Padding(3).Text("Menge");
                                      });
                            td.ColumnsDefinition(tcdd =>
                                                 {
                                                     tcdd.RelativeColumn();
                                                     tcdd.RelativeColumn();
                                                 });

                            foreach (var spool in spoolList.Spools)
                            {
                                td.Cell().ColumnSpan(2).BorderBottom(0.5f);
                                td.Cell().Padding(3).Text(spool.Name);
                                td.Cell().Padding(3).Text(spool.Amount.ToString(CultureInfo.CurrentUICulture));
                            }
                        });

                    if (lineSet && page.Spools.Count == 1) continue;
                    cd.AutoItem().BorderRight(0.5f);
                }
            });
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
        pd.Header()
          .PaddingBottom(1, Unit.Centimetre)
          .Column(
            cd =>
            {
                cd.Item().Text(
                    td => { td.Span($"Bestellung: {page.Id} - {page.Time:D}").Bold().FontSize(10); });
            });
    }
}