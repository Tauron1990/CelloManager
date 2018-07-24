using System;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Tauron.Application.CelloManager.Logic.RefillPrinter.Rule
{
    public static class PrintHelper
    {
        
        public static bool PrintOrder(FlowDocument flowDocument, string name, Action<string> selectPrinter)
        {
            bool temp = false;

            UiSynchronize.Synchronize.Invoke(() =>
                                             {
                                                 var pd = new PrintDialog();
                                                 bool ok = false;

                                                 if(!string.IsNullOrWhiteSpace(name))
                                                 {
                                                     try
                                                     {
                                                         var server = new LocalPrintServer();
                                                         var              queue  = server.GetPrintQueues().FirstOrDefault(q => q.Name == name);
                                                         if (queue != null && !queue.IsOffline)
                                                         {
                                                             pd.PrintQueue            = queue;
                                                             pd.PrintTicket.CopyCount = 1;
                                                             ok                       = true;
                                                         }
                                                     }
                                                     catch (PrintSystemException)
                                                     {
                                                     }                             
                                                 }

                                                 if (!ok)
                                                 {
                                                     temp = pd.ShowDialog() == true;
                                                     if (temp)
                                                         selectPrinter(pd.PrintQueue.Name);
                                                 }
                                                 else temp = true;

                                                 if (!temp) return;

                                                 flowDocument.PageHeight  = pd.PrintableAreaHeight;
                                                 flowDocument.PageWidth   = pd.PrintableAreaWidth;
                                                 flowDocument.PagePadding = new Thickness(50);
                                                 flowDocument.ColumnGap   = 0;
                                                 flowDocument.ColumnWidth = pd.PrintableAreaWidth;

                                                 IDocumentPaginatorSource dps = flowDocument;
                                                 pd.PrintDocument(dps.DocumentPaginator, "Cello Spool Order Document");
                                             });


            return temp;
        }
    }
}