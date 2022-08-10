using System.Threading.Tasks;

namespace AssetManager.ViewModels;

public sealed class ScriptsViewModel : DataEntryViewModel
{
    protected override Task<DataEntry> AddEntry(string rootDic) => throw new System.NotImplementedException();

    protected override Task EditContent(DataEntry entry) => throw new System.NotImplementedException();

    protected override string ResolveFilePath(string root) => throw new System.NotImplementedException();
}