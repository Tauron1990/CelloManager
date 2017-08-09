﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using JetBrains.Annotations;
using Tauron.Application.CelloManager.Data.Historie;
using Tauron.Application.CelloManager.Resources;

namespace Tauron.Application.CelloManager.UI.PrintOrder
{
    [PublicAPI]
    public static class PrintHelper
    {
        public class HelperTableGroup
        {
            public HelperTableGroup()
            {
                LeftSpools = new List<CommittedSpool>();
                RightSpools = new List<CommittedSpool>();
            }

            public string LeftType { get; set; }
            public List<CommittedSpool> LeftSpools { get; }

            public string RightType { get; set; }
            public List<CommittedSpool> RightSpools { get; }
        }

        public static bool PrintOrder(CommittedRefill refill)
        {
            bool temp = false;

            UiSynchronize.Synchronize.Invoke(() =>
            {

                var fd = BuildFlowDocument(refill);
                var pd = new PrintDialog();

                temp = pd.ShowDialog() == true;

                fd.PageHeight = pd.PrintableAreaHeight;
                fd.PageWidth = pd.PrintableAreaWidth;
                fd.PagePadding = new Thickness(50);
                fd.ColumnGap = 0;
                fd.ColumnWidth = pd.PrintableAreaWidth;

                IDocumentPaginatorSource dps = fd;
                pd.PrintDocument(dps.DocumentPaginator, "flow doc");
            });

            return temp;
        }

        public static FlowDocument BuildFlowDocument(CommittedRefill refill)
        {
            var document = new FlowDocument();

            BuildHeader(document);
            BuildTable(document, refill);
            BuildFooter(document, refill);

            return document;
        }

        public static void BuildFooter(FlowDocument document, CommittedRefill refill)
        {
            document.Blocks.Add(new Paragraph(new Run(refill.SentTime.ToString(CultureInfo.CurrentCulture))));
        }

        public static void BuildHeader(FlowDocument document)
        {
            var header = new Paragraph(
                new Bold(
                    new Run
                    {
                        Text = UIResources.PrintOrderHeaderText
                    }))
            {
                TextAlignment = TextAlignment.Center
            };

            document.Blocks.Add(header);
        }

        public static void BuildTable(FlowDocument document, CommittedRefill refill)
        {
            var groups = BuildGroups(refill);

            var section = new Section();
            var table = new Table
            {
                BorderBrush = new SolidColorBrush {Color = Colors.Black},
                BorderThickness = new Thickness(1)
            };

            foreach (var tableGroup in groups)
            {
                var rowGroup = new TableRowGroup();

                var row = new TableRow();

                if (!string.IsNullOrEmpty(tableGroup.LeftType))
                {
                    var cell1 = new TableCell(new Paragraph(new Bold(new Run {Text = tableGroup.LeftType + ":"})));
                    row.Cells.Add(cell1);
                }

                if (!string.IsNullOrEmpty(tableGroup.RightType))
                {
                    var cell2 = new TableCell(new Paragraph(new Bold(new Run {Text = tableGroup.RightType + ":"})));
                    row.Cells.Add(cell2);
                }

                rowGroup.Rows.Add(row);

                var val = Math.Max(tableGroup.LeftSpools.Count, tableGroup.RightSpools.Count);

                for (var i = 0; i < val; i++)
                {
                    var newRow = new TableRow();

                    CommittedSpool leftSpool = null;
                    CommittedSpool rightSpool = null;

                    if (tableGroup.LeftSpools.Count > i)
                        leftSpool = tableGroup.LeftSpools[i];
                    if (tableGroup.RightSpools.Count > i)
                        rightSpool = tableGroup.RightSpools[i];

                    var leftCell = new TableCell();
                    var rightCell = new TableCell();

                    if (leftSpool != null)
                        FormatCell(leftSpool, leftCell);
                    if (rightSpool != null)
                        FormatCell(rightSpool, rightCell);

                    newRow.Cells.Add(leftCell);
                    newRow.Cells.Add(rightCell);

                    rowGroup.Rows.Add(newRow);
                }

                table.RowGroups.Add(rowGroup);
            }

            section.Blocks.Add(table);
            document.Blocks.Add(section);
        }

        public static void FormatCell(CommittedSpool spool, TableCell cell)
        {
            var name = spool.Name;
            var count = spool.OrderedCount.ToString();

            var paragraph = new Paragraph();

            var run1 = new Run(name);
            var run2 = new Run(" - ");
            var run3 = new Run(count);

            paragraph.Inlines.Add(run1);
            paragraph.Inlines.Add(run2);
            paragraph.Inlines.Add(run3);

            cell.Blocks.Add(paragraph);
        }

        public static IEnumerable<HelperTableGroup> BuildGroups(CommittedRefill refill)
        {
            var types = new HashSet<string>();
            var groups = new List<HelperTableGroup>();

            foreach (var spool in refill.CommitedSpools)
                if (types.Add(spool.Type))
                {
                    var ok = false;
                    foreach (var tgroup in groups.Where(tgroup => string.IsNullOrEmpty(tgroup.RightType)))
                    {
                        tgroup.RightType = spool.Type;
                        tgroup.RightSpools.Add(spool);
                        ok = true;
                        break;
                    }

                    if (ok) continue;

                    var newGroup = new HelperTableGroup {LeftType = spool.Type};

                    newGroup.LeftSpools.Add(spool);

                    groups.Add(newGroup);
                }
                else
                {
                    foreach (var helperTableGroup in groups)
                    {
                        if (helperTableGroup.LeftType == spool.Type)
                        {
                            helperTableGroup.LeftSpools.Add(spool);
                            break;
                        }
                        if (helperTableGroup.RightType != spool.Type) continue;

                        helperTableGroup.RightSpools.Add(spool);
                        break;
                    }
                }

            return groups;
        }
    }
}