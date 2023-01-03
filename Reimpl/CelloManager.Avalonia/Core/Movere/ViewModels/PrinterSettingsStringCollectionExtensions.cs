﻿using System.Collections.Generic;
using System.Drawing.Printing;

namespace CelloManager.Core.Movere.ViewModels
{
    internal static class PrinterSettingsStringCollectionExtensions
    {
        public static IReadOnlyList<string> ToReadOnlyList(this PrinterSettings.StringCollection collection)
        {
            var printers = new string[collection.Count];
            collection.CopyTo(printers, 0);

            return printers;
        }
    }
}
