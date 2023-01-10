using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CelloManager.Core.Logic;

public static class ReadySpoolSorter
{
    private sealed class NameComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (ReferenceEquals(x, y))
                return 0;
            if (x is null)
                return -1;
            if (y is null)
                return 1;

            double digit1 = ExtractDigit(x);
            double digit2 = ExtractDigit(y);

            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (digit1 != -1 && digit2 != -1)
                // ReSharper restore CompareOfFloatsByEqualityOperator
                return digit2.CompareTo(digit1);

            // ReSharper disable once StringCompareToIsCultureSpecific
            return x.CompareTo(y);
        }

        private static double ExtractDigit(string name)
        {
            int count = name.Count(char.IsDigit);
            if (count == 0) return -1;

            if(double.TryParse(name.AsSpan()[..count], NumberStyles.Any, CultureInfo.CurrentUICulture, out double result))
                return result;

            return -1;
        }
    }

    public static readonly IComparer<string> NameSorter = new NameComparer();

    public static readonly IComparer<ReadySpoolModel> ModelSorter = Comparer<ReadySpoolModel>.Create(Comparison);

    private static int Comparison(ReadySpoolModel x, ReadySpoolModel y)
        => NameSorter.Compare(x.Name, y.Name);

    public static readonly IComparer<string> CategorySorter = StringComparer.CurrentCulture;
    
    public static readonly IComparer<ReadySpoolModel> CategoryModelSorter = Comparer<ReadySpoolModel>.Create(CatComparison);

    private static int CatComparison(ReadySpoolModel x, ReadySpoolModel y)
        => CategorySorter.Compare(x.Category, y.Category);
}