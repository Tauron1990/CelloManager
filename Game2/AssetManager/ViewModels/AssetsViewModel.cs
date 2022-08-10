using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using ReactiveUI;

namespace AssetManager.ViewModels;



public sealed class AssetsViewModel : DataEntryViewModel
{
    protected override Task<DataEntry> AddEntry(string rootDic) => throw new NotImplementedException();

    protected override Task EditContent(DataEntry entry) => throw new NotImplementedException();

    protected override string ResolveFilePath(string root) => throw new NotImplementedException();
}