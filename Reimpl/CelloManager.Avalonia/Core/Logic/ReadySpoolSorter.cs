using System;
using System.Collections.Generic;
using System.Linq;

namespace CelloManager.Core.Logic;

public static class ReadySpoolSorter
{
    private sealed class CategoryComparer : IComparer<ReadySpoolModel>
    {
        public int Compare(ReadySpoolModel? x, ReadySpoolModel? y)
        {
            if (ReferenceEquals(x, y))
                return 0;
            if (x == null)
                return -1;

            // ReSharper disable once StringCompareToIsCultureSpecific
            return x.Category.CompareTo(y?.Category);
        }
    }
    
    private sealed class NameComparer : IComparer<ReadySpoolModel>
    {
        public int Compare(ReadySpoolModel? x, ReadySpoolModel? y)
        {
            if (ReferenceEquals(x, y))
                return 0;
            if (x is null)
                return -1;
            if (y is null)
                return 1;

            var digit1 = ExtractDigit(x.Name);
            var digit2 = ExtractDigit(y.Name);

            if (digit1 != -1 && digit2 != -1)
                return digit2.CompareTo(digit1);

            // ReSharper disable once StringCompareToIsCultureSpecific
            return x.Name.CompareTo(y.Name);
        }

        private int ExtractDigit(string name)
        {
            var count = name.Count(char.IsDigit);
            if (count == -1) return -1;

            return int.Parse(name.AsSpan()[..count]);
        }
    }

    public static readonly IComparer<ReadySpoolModel> NameSorter = new NameComparer();

    public static readonly IComparer<ReadySpoolModel> CategorySorter = new CategoryComparer();
}